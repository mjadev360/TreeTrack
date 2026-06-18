<script setup lang="ts">
import { ref, watch } from 'vue'
import type { IssuePriority, IssueStatus, IssueType } from '@/types/issue'
import { ISSUE_PRIORITIES, ISSUE_STATUSES, ISSUE_TYPES } from '@/types/issue'

const props = defineProps<{
  open: boolean
  parentIssueId?: number | null
}>()

const emit = defineEmits<{
  close: []
  save: [payload: {
    title: string
    type: IssueType
    status: IssueStatus
    priority: IssuePriority
    assignee: string | null
    dueDate: string | null
    description: string | null
    parentIssueId?: number | null
  }]
}>()

const title = ref('')
const type = ref<IssueType>('task')
const status = ref<IssueStatus>('open')
const priority = ref<IssuePriority>('medium')
const assignee = ref('')
const dueDate = ref('')
const description = ref('')
const saving = ref(false)

watch(
  () => props.open,
  (isOpen) => {
    if (!isOpen) return

    title.value = ''
    type.value = 'task'
    status.value = 'open'
    priority.value = 'medium'
    assignee.value = ''
    dueDate.value = ''
    description.value = ''
  }
)

async function handleSave() {
  if (!title.value.trim()) return

  saving.value = true
  try {
    emit('save', {
      title: title.value.trim(),
      type: type.value,
      status: status.value,
      priority: priority.value,
      assignee: assignee.value.trim() || null,
      dueDate: dueDate.value || null,
      description: description.value.trim() || null,
      parentIssueId: props.parentIssueId
    })
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div v-if="open" class="modal-overlay" @click.self="emit('close')">
    <div class="modal">
      <div class="modal-header">
        {{ parentIssueId ? 'New Sub-issue' : 'New Issue' }}
      </div>
      <div class="modal-body">
        <div class="form-field">
          <label class="form-label">Title</label>
          <input v-model="title" class="form-input" placeholder="Issue title" />
        </div>

        <div class="form-row">
          <div class="form-field">
            <label class="form-label">Type</label>
            <select v-model="type" class="form-select">
              <option v-for="t in ISSUE_TYPES" :key="t" :value="t">{{ t }}</option>
            </select>
          </div>
          <div class="form-field">
            <label class="form-label">Priority</label>
            <select v-model="priority" class="form-select">
              <option v-for="p in ISSUE_PRIORITIES" :key="p" :value="p">{{ p }}</option>
            </select>
          </div>
        </div>

        <div class="form-row">
          <div class="form-field">
            <label class="form-label">Assignee</label>
            <input v-model="assignee" class="form-input" placeholder="e.g. SR" />
          </div>
          <div class="form-field">
            <label class="form-label">Due Date</label>
            <input v-model="dueDate" type="date" class="form-input" />
          </div>
        </div>

        <div class="form-field">
          <label class="form-label">Description</label>
          <textarea v-model="description" class="form-textarea" placeholder="Description..." />
        </div>
      </div>
      <div class="modal-footer">
        <button class="btn" @click="emit('close')">Cancel</button>
        <button class="btn btn-primary" :disabled="!title.trim() || saving" @click="handleSave">
          {{ saving ? 'Creating...' : 'Create' }}
        </button>
      </div>
    </div>
  </div>
</template>
