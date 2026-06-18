<script setup lang="ts">
import { ref, watch } from 'vue'
import { useIssueStore } from '@/stores/issueStore'
import type { IssuePriority, IssueStatus, IssueType } from '@/types/issue'
import { ISSUE_PRIORITIES, ISSUE_STATUSES, ISSUE_TYPES } from '@/types/issue'

const emit = defineEmits<{
  subIssue: []
  delete: []
}>()

const issueStore = useIssueStore()

const title = ref('')
const type = ref<IssueType>('task')
const status = ref<IssueStatus>('open')
const priority = ref<IssuePriority>('medium')
const assignee = ref('')
const dueDate = ref('')
const description = ref('')
const saving = ref(false)
const saveError = ref<string | null>(null)

let saveTimeout: ReturnType<typeof setTimeout> | null = null
let loadedIssueId: number | null = null

function syncFromSelected() {
  const issue = issueStore.selectedIssue
  if (!issue) return

  loadedIssueId = issue.id
  title.value = issue.title
  type.value = issue.type
  status.value = issue.status
  priority.value = issue.priority
  assignee.value = issue.assignee ?? ''
  dueDate.value = issue.dueDate ?? ''
  description.value = issue.description ?? ''
  saveError.value = null
}

watch(
  () => issueStore.selectedId,
  () => {
    if (saveTimeout) {
      clearTimeout(saveTimeout)
      saveTimeout = null
    }
    syncFromSelected()
  },
  { immediate: true }
)

function buildPayload() {
  return {
    title: title.value.trim(),
    type: type.value,
    status: status.value,
    priority: priority.value,
    assignee: assignee.value.trim() || null,
    dueDate: dueDate.value || null,
    description: description.value.trim() || null
  }
}

function hasChanges() {
  const issue = issueStore.selectedIssue
  if (!issue) return false

  const payload = buildPayload()
  return (
    payload.title !== issue.title ||
    payload.type !== issue.type ||
    payload.status !== issue.status ||
    payload.priority !== issue.priority ||
    payload.assignee !== issue.assignee ||
    payload.dueDate !== issue.dueDate ||
    payload.description !== issue.description
  )
}

async function save() {
  const issue = issueStore.selectedIssue
  if (!issue || saving.value || loadedIssueId !== issue.id) return
  if (!hasChanges()) return

  const payload = buildPayload()
  if (!payload.title) return

  saving.value = true
  saveError.value = null
  try {
    await issueStore.updateIssue(issue.id, payload)
  } catch {
    saveError.value = issueStore.error ?? 'Failed to save'
  } finally {
    saving.value = false
  }
}

function scheduleSave() {
  if (saveTimeout) clearTimeout(saveTimeout)
  saveTimeout = setTimeout(save, 600)
}

function saveNow() {
  if (saveTimeout) clearTimeout(saveTimeout)
  save()
}

watch([type, status, priority, dueDate], saveNow)
watch([title, assignee, description], scheduleSave)
</script>

<template>
  <div class="detail">
    <template v-if="issueStore.selectedIssue">
      <div class="detail-header">
        <div class="detail-header-main">
          <div class="detail-id">
            {{ issueStore.selectedIssue.key }} · {{ issueStore.selectedIssue.type }}
          </div>
          <input
            v-model="title"
            class="detail-title-input"
            placeholder="Issue title"
          />
        </div>
        <button class="detail-close" @click="issueStore.closeDetail()">✕</button>
      </div>

      <div class="detail-body">
        <div class="form-row">
          <div class="form-field">
            <label class="form-label">Type</label>
            <select v-model="type" class="form-select">
              <option v-for="t in ISSUE_TYPES" :key="t" :value="t">{{ t }}</option>
            </select>
          </div>
          <div class="form-field">
            <label class="form-label">Status</label>
            <select v-model="status" class="form-select">
              <option v-for="s in ISSUE_STATUSES" :key="s" :value="s">{{ s }}</option>
            </select>
          </div>
        </div>

        <div class="form-row">
          <div class="form-field">
            <label class="form-label">Priority</label>
            <select v-model="priority" class="form-select">
              <option v-for="p in ISSUE_PRIORITIES" :key="p" :value="p">{{ p }}</option>
            </select>
          </div>
          <div class="form-field">
            <label class="form-label">Assignee</label>
            <input v-model="assignee" class="form-input" placeholder="e.g. SR" />
          </div>
        </div>

        <div class="form-field">
          <label class="form-label">Due Date</label>
          <input v-model="dueDate" type="date" class="form-input" />
        </div>

        <div class="form-field">
          <label class="form-label">Description</label>
          <textarea
            v-model="description"
            class="form-textarea"
            placeholder="Description..."
          />
        </div>
      </div>

      <div class="detail-actions">
        <span v-if="saving" class="detail-save-status">Saving...</span>
        <span v-else-if="saveError" class="detail-save-status error">{{ saveError }}</span>
        <span v-else-if="hasChanges()" class="detail-save-status">Unsaved changes</span>
        <div class="detail-action-buttons">
          <button class="btn" @click="emit('subIssue')">+ Sub-issue</button>
          <button class="btn btn-danger" @click="emit('delete')">Delete</button>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.detail-header-main {
  flex: 1;
  min-width: 0;
}

.detail-title-input {
  width: 100%;
  background: transparent;
  border: 1px solid transparent;
  border-radius: 4px;
  padding: 2px 4px;
  margin: 0 -4px;
  font-family: inherit;
  font-size: 13px;
  font-weight: 500;
  line-height: 1.4;
  color: var(--text);
  outline: none;
}

.detail-title-input:hover {
  border-color: var(--border2);
}

.detail-title-input:focus {
  border-color: var(--accent);
  background: var(--surface2);
}

.detail-actions {
  align-items: center;
}

.detail-save-status {
  font-size: 10px;
  color: var(--text-muted);
  margin-right: auto;
}

.detail-save-status.error {
  color: var(--red);
}

.detail-action-buttons {
  display: flex;
  gap: 6px;
  margin-left: auto;
}
</style>
