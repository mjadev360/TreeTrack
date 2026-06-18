import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { issueService } from '@/services/issueService'
import type {
  CreateIssueRequest,
  IssueDetail,
  IssueTreeNode,
  UpdateIssueRequest
} from '@/types/issue'

function findNode(nodes: IssueTreeNode[], id: number): IssueTreeNode | null {
  for (const node of nodes) {
    if (node.id === id) return node
    const found = findNode(node.children, id)
    if (found) return found
  }
  return null
}

function applyIssueFields(node: IssueTreeNode, fields: Partial<IssueTreeNode>) {
  if (fields.key !== undefined) node.key = fields.key
  if (fields.type !== undefined) node.type = fields.type
  if (fields.title !== undefined) node.title = fields.title
  if (fields.status !== undefined) node.status = fields.status
  if (fields.priority !== undefined) node.priority = fields.priority
  if (fields.assignee !== undefined) node.assignee = fields.assignee
  if (fields.dueDate !== undefined) node.dueDate = fields.dueDate
  if (fields.description !== undefined) node.description = fields.description
}

function detailToTreeNode(detail: IssueDetail): IssueTreeNode {
  return {
    id: detail.id,
    key: detail.key,
    type: detail.type,
    title: detail.title,
    status: detail.status,
    priority: detail.priority,
    assignee: detail.assignee,
    dueDate: detail.dueDate,
    description: detail.description,
    children: []
  }
}

function insertIssueNode(
  nodes: IssueTreeNode[],
  node: IssueTreeNode,
  parentId: number | null
): boolean {
  if (parentId === null) {
    nodes.push(node)
    return true
  }

  const parent = findNode(nodes, parentId)
  if (!parent) return false

  parent.children.push(node)
  return true
}

function removeIssueNode(nodes: IssueTreeNode[], id: number): boolean {
  const index = nodes.findIndex((node) => node.id === id)
  if (index !== -1) {
    nodes.splice(index, 1)
    return true
  }

  for (const node of nodes) {
    if (removeIssueNode(node.children, id)) return true
  }

  return false
}

function flattenTree(nodes: IssueTreeNode[]): IssueTreeNode[] {
  const list: IssueTreeNode[] = []
  for (const node of nodes) {
    list.push(node)
    if (node.children.length > 0) {
      list.push(...flattenTree(node.children))
    }
  }
  return list
}

function nodeMatchesFilters(
  node: IssueTreeNode,
  statusFilter: string,
  priorityFilter: string,
  searchQuery: string
): boolean {
  const statusMatch = statusFilter === 'all' || node.status === statusFilter
  const priorityMatch = priorityFilter === 'all' || node.priority === priorityFilter
  const searchMatch =
    !searchQuery ||
    node.title.toLowerCase().includes(searchQuery) ||
    node.key.toLowerCase().includes(searchQuery) ||
    (node.assignee?.toLowerCase().includes(searchQuery) ?? false)

  return statusMatch && priorityMatch && searchMatch
}

function filterTree(
  nodes: IssueTreeNode[],
  statusFilter: string,
  priorityFilter: string,
  searchQuery: string
): IssueTreeNode[] {
  const result: IssueTreeNode[] = []

  for (const node of nodes) {
    const filteredChildren = filterTree(node.children, statusFilter, priorityFilter, searchQuery)
    const selfMatches = nodeMatchesFilters(node, statusFilter, priorityFilter, searchQuery)

    if (selfMatches || filteredChildren.length > 0) {
      result.push({ ...node, children: filteredChildren })
    }
  }

  return result
}

export const useIssueStore = defineStore('issues', () => {
  const projectId = ref<number | null>(null)
  const issues = ref<IssueTreeNode[]>([])
  const selectedId = ref<number | null>(null)
  const expandedIds = ref<Set<number>>(new Set())
  const statusFilter = ref<string>('all')
  const priorityFilter = ref<string>('all')
  const searchQuery = ref('')
  const loading = ref(false)
  const error = ref<string | null>(null)

  const flatIssues = computed(() => flattenTree(issues.value))

  const filteredTree = computed(() =>
    filterTree(
      issues.value,
      statusFilter.value,
      priorityFilter.value,
      searchQuery.value.trim().toLowerCase()
    )
  )

  const selectedIssue = computed(() =>
    selectedId.value === null
      ? null
      : flatIssues.value.find((issue) => issue.id === selectedId.value) ?? null
  )

  const stats = computed(() => {
    const all = flatIssues.value
    return {
      total: all.length,
      open: all.filter((n) => n.status === 'open').length,
      inProgress: all.filter((n) => n.status === 'in-progress').length,
      blocked: all.filter((n) => n.status === 'blocked').length,
      closed: all.filter((n) => n.status === 'closed').length,
      byType: {
        epic: all.filter((n) => n.type === 'epic').length,
        story: all.filter((n) => n.type === 'story').length,
        task: all.filter((n) => n.type === 'task').length,
        bug: all.filter((n) => n.type === 'bug').length,
        subtask: all.filter((n) => n.type === 'subtask').length
      }
    }
  })

  function resetState() {
    issues.value = []
    selectedId.value = null
    expandedIds.value = new Set()
    statusFilter.value = 'all'
    priorityFilter.value = 'all'
    searchQuery.value = ''
    error.value = null
  }

  function initializeExpanded() {
    const next = new Set<number>()
    for (const issue of issues.value) {
      next.add(issue.id)
    }
    expandedIds.value = next
  }

  async function fetchTree(id: number) {
    if (projectId.value !== id) {
      resetState()
      projectId.value = id
    }

    loading.value = true
    error.value = null
    try {
      issues.value = await issueService.getTree(id)
      if (expandedIds.value.size === 0) {
        initializeExpanded()
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load issues'
      throw err
    } finally {
      loading.value = false
    }
  }

  function selectIssue(id: number) {
    selectedId.value = id
  }

  function closeDetail() {
    selectedId.value = null
  }

  function toggleExpand(id: number) {
    const next = new Set(expandedIds.value)
    if (next.has(id)) {
      next.delete(id)
    } else {
      next.add(id)
    }
    expandedIds.value = next
  }

  function expandAll() {
    expandedIds.value = new Set(flatIssues.value.map((issue) => issue.id))
  }

  function collapseAll() {
    const next = new Set<number>()
    for (const issue of issues.value) {
      next.add(issue.id)
    }
    expandedIds.value = next
  }

  function setStatusFilter(filter: string) {
    statusFilter.value = filter
  }

  function setPriorityFilter(filter: string) {
    priorityFilter.value = filter
  }

  function setSearchQuery(query: string) {
    searchQuery.value = query
    if (query.trim()) {
      expandAll()
    }
  }

  async function createIssue(request: CreateIssueRequest) {
    if (projectId.value === null) {
      throw new Error('No project selected')
    }

    error.value = null
    const created = await issueService.create(projectId.value, request)
    const node = detailToTreeNode(created)
    const inserted = insertIssueNode(issues.value, node, created.parentIssueId ?? null)
    if (!inserted) {
      await fetchTree(projectId.value)
    }
    selectedId.value = created.id
    expandedIds.value = new Set([...expandedIds.value, created.id])
    if (created.parentIssueId) {
      expandedIds.value = new Set([...expandedIds.value, created.parentIssueId])
    }
    return created
  }

  async function updateIssue(id: number, request: UpdateIssueRequest) {
    if (projectId.value === null) {
      throw new Error('No project selected')
    }

    error.value = null
    const updated = await issueService.update(projectId.value, id, request)
    const node = findNode(issues.value, id)
    if (node) {
      applyIssueFields(node, {
        key: updated.key,
        type: updated.type,
        title: updated.title,
        status: updated.status,
        priority: updated.priority,
        assignee: updated.assignee,
        dueDate: updated.dueDate,
        description: updated.description
      })
    }
    selectedId.value = updated.id
    return updated
  }

  async function deleteIssue(id: number) {
    if (projectId.value === null) {
      throw new Error('No project selected')
    }

    error.value = null
    await issueService.delete(projectId.value, id)
    if (selectedId.value === id) {
      selectedId.value = null
    }
    const removed = removeIssueNode(issues.value, id)
    if (!removed) {
      await fetchTree(projectId.value)
    }
  }

  return {
    projectId,
    issues,
    selectedId,
    expandedIds,
    statusFilter,
    priorityFilter,
    searchQuery,
    loading,
    error,
    flatIssues,
    filteredTree,
    selectedIssue,
    stats,
    fetchTree,
    selectIssue,
    closeDetail,
    toggleExpand,
    expandAll,
    collapseAll,
    setStatusFilter,
    setPriorityFilter,
    setSearchQuery,
    createIssue,
    updateIssue,
    deleteIssue,
    resetState
  }
})
