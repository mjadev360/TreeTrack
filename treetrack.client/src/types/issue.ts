export type IssueType = 'epic' | 'story' | 'task' | 'bug' | 'subtask'
export type IssueStatus = 'open' | 'in-progress' | 'review' | 'blocked' | 'closed'
export type IssuePriority = 'critical' | 'high' | 'medium' | 'low'

export interface IssueTreeNode {
  id: number
  key: string
  type: IssueType
  title: string
  status: IssueStatus
  priority: IssuePriority
  assignee: string | null
  dueDate: string | null
  description: string | null
  children: IssueTreeNode[]
}

export interface IssueDetail {
  id: number
  key: string
  parentIssueId: number | null
  type: IssueType
  title: string
  status: IssueStatus
  priority: IssuePriority
  assignee: string | null
  dueDate: string | null
  description: string | null
}

export interface CreateIssueRequest {
  title: string
  type: IssueType
  parentIssueId?: number | null
  status?: IssueStatus
  priority?: IssuePriority
  assignee?: string | null
  dueDate?: string | null
  description?: string | null
}

export interface UpdateIssueRequest {
  title: string
  type: IssueType
  status: IssueStatus
  priority: IssuePriority
  assignee?: string | null
  dueDate?: string | null
  description?: string | null
}

export const ISSUE_TYPES: IssueType[] = ['epic', 'story', 'task', 'bug', 'subtask']
export const ISSUE_STATUSES: IssueStatus[] = ['open', 'in-progress', 'review', 'blocked', 'closed']
export const ISSUE_PRIORITIES: IssuePriority[] = ['critical', 'high', 'medium', 'low']

export const STATUS_LABELS: Record<IssueStatus, string> = {
  open: 'Open',
  'in-progress': 'In Progress',
  review: 'Review',
  blocked: 'Blocked',
  closed: 'Closed'
}

export const TYPE_ICONS: Record<IssueType, { icon: string; class: string }> = {
  epic: { icon: '⬡', class: 'icon-epic' },
  story: { icon: '◈', class: 'icon-story' },
  task: { icon: '◻', class: 'icon-task' },
  bug: { icon: '⬤', class: 'icon-bug' },
  subtask: { icon: '◦', class: 'icon-subtask' }
}

export const AVATARS: Record<string, { color: string; bg: string; label: string }> = {
  SR: { color: '#4fffb0', bg: 'rgba(79,255,176,0.15)', label: 'SR' },
  LK: { color: '#60a5fa', bg: 'rgba(96,165,250,0.15)', label: 'LK' },
  MJ: { color: '#a78bfa', bg: 'rgba(167,139,250,0.15)', label: 'MJ' },
  DT: { color: '#ffd166', bg: 'rgba(255,209,102,0.15)', label: 'DT' }
}

export function formatDueDate(dueDate: string | null): string {
  if (!dueDate) return '—'
  const date = new Date(`${dueDate}T00:00:00`)
  return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
}

export function formatDueDateLong(dueDate: string | null): string {
  if (!dueDate) return 'No due date'
  return formatDueDate(dueDate)
}
