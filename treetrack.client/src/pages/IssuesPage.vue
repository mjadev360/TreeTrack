<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useIssueStore } from '@/stores/issueStore'
import { useProjectStore } from '@/stores/projectStore'
import type { IssueDetail, IssuePriority, IssueStatus, IssueType } from '@/types/issue'
import '@/assets/issue-tracker.css'
import IssueTopBar from '@/components/issues/IssueTopBar.vue'
import IssueSidebar from '@/components/issues/IssueSidebar.vue'
import IssueSearchBar from '@/components/issues/IssueSearchBar.vue'
import IssueTreeView from '@/components/issues/IssueTreeView.vue'
import IssueDetailPanel from '@/components/issues/IssueDetailPanel.vue'
import IssueStatusBar from '@/components/issues/IssueStatusBar.vue'
import IssueFormModal from '@/components/issues/IssueFormModal.vue'

const route = useRoute()
const router = useRouter()
const issueStore = useIssueStore()
const projectStore = useProjectStore()

const sidebarCollapsed = ref(false)
const modalOpen = ref(false)
const modalMode = ref<'create' | 'edit'>('create')
const editingIssue = ref<IssueDetail | null>(null)
const parentIssueId = ref<number | null>(null)
const loadError = ref<string | null>(null)

function getProjectId(): number | null {
  const raw = route.params.projectId
  const id = Number(Array.isArray(raw) ? raw[0] : raw)
  return Number.isFinite(id) ? id : null
}

async function loadProjectData() {
  const projectId = getProjectId()
  if (projectId === null) {
    loadError.value = 'Invalid project'
    return
  }

  loadError.value = null
  try {
    await projectStore.fetchById(projectId)
    await issueStore.fetchTree(projectId)
  } catch {
    loadError.value = projectStore.error ?? issueStore.error ?? 'Failed to load project'
    router.replace({ name: 'Workspace' })
  }
}

onMounted(() => {
  loadProjectData()
})

watch(
  () => route.params.projectId,
  () => {
    loadProjectData()
  }
)

function openNewIssue() {
  modalMode.value = 'create'
  editingIssue.value = null
  parentIssueId.value = null
  modalOpen.value = true
}

function openEdit() {
  if (!issueStore.selectedIssue) return
  modalMode.value = 'edit'
  editingIssue.value = {
    id: issueStore.selectedIssue.id,
    key: issueStore.selectedIssue.key,
    parentIssueId: null,
    type: issueStore.selectedIssue.type,
    title: issueStore.selectedIssue.title,
    status: issueStore.selectedIssue.status,
    priority: issueStore.selectedIssue.priority,
    assignee: issueStore.selectedIssue.assignee,
    dueDate: issueStore.selectedIssue.dueDate,
    description: issueStore.selectedIssue.description
  }
  parentIssueId.value = null
  modalOpen.value = true
}

function openSubIssue() {
  if (!issueStore.selectedIssue) return
  modalMode.value = 'create'
  editingIssue.value = null
  parentIssueId.value = issueStore.selectedIssue.id
  modalOpen.value = true
}

async function handleSave(payload: {
  title: string
  type: IssueType
  status: IssueStatus
  priority: IssuePriority
  assignee: string | null
  dueDate: string | null
  description: string | null
  parentIssueId?: number | null
}) {
  try {
    if (modalMode.value === 'create') {
      await issueStore.createIssue({
        title: payload.title,
        type: payload.type,
        parentIssueId: payload.parentIssueId,
        status: payload.status,
        priority: payload.priority,
        assignee: payload.assignee,
        dueDate: payload.dueDate,
        description: payload.description
      })
    } else if (editingIssue.value) {
      await issueStore.updateIssue(editingIssue.value.id, {
        title: payload.title,
        type: payload.type,
        status: payload.status,
        priority: payload.priority,
        assignee: payload.assignee,
        dueDate: payload.dueDate,
        description: payload.description
      })
    }
    modalOpen.value = false
  } catch {
    // error surfaced via store
  }
}

async function handleDelete() {
  if (!issueStore.selectedIssue) return
  const confirmed = window.confirm(
    `Delete ${issueStore.selectedIssue.key} and all sub-issues? This cannot be undone.`
  )
  if (!confirmed) return

  try {
    await issueStore.deleteIssue(issueStore.selectedIssue.id)
  } catch {
    // error surfaced via store
  }
}
</script>

<template>
  <div v-if="loadError" class="issue-tracker issue-error">
    <p>{{ loadError }}</p>
    <router-link to="/workspace">Back to Workspace</router-link>
  </div>
  <div v-else class="issue-tracker">
    <IssueTopBar @new-issue="openNewIssue" />

    <div class="layout">
      <div class="sidebar-wrapper" :class="{ collapsed: sidebarCollapsed }">
        <IssueSidebar />
      </div>
      <button
        class="sidebar-toggle"
        :class="{ collapsed: sidebarCollapsed }"
        :title="sidebarCollapsed ? 'Show sidebar' : 'Hide sidebar'"
        @click="sidebarCollapsed = !sidebarCollapsed"
      >
        {{ sidebarCollapsed ? '▶' : '◀' }}
      </button>

      <div class="main">
        <IssueSearchBar />
        <IssueTreeView />
      </div>

      <IssueDetailPanel
        @edit="openEdit"
        @sub-issue="openSubIssue"
        @delete="handleDelete"
      />
    </div>

    <IssueStatusBar />

    <IssueFormModal
      :open="modalOpen"
      :mode="modalMode"
      :issue="editingIssue"
      :parent-issue-id="parentIssueId"
      @close="modalOpen = false"
      @save="handleSave"
    />
  </div>
</template>

<style scoped>
.issue-error {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 16px;
  color: var(--text-muted, #888);
}
</style>
