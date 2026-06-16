<script setup lang="ts">
import { useIssueStore } from '@/stores/issueStore'
import { formatDueDateLong } from '@/types/issue'
import IssueBadge from './IssueBadge.vue'
import IssuePriority from './IssuePriority.vue'
import AssigneeAvatar from './AssigneeAvatar.vue'

const emit = defineEmits<{
  edit: []
  subIssue: []
  delete: []
}>()

const issueStore = useIssueStore()
</script>

<template>
  <div class="detail" :class="{ hidden: !issueStore.selectedIssue }">
    <template v-if="issueStore.selectedIssue">
      <div class="detail-header">
        <div>
          <div class="detail-id">
            {{ issueStore.selectedIssue.key }} · {{ issueStore.selectedIssue.type }}
          </div>
          <div class="detail-title">{{ issueStore.selectedIssue.title }}</div>
        </div>
        <button class="detail-close" @click="issueStore.closeDetail()">✕</button>
      </div>

      <div class="detail-body">
        <div class="detail-field">
          <div class="detail-field-label">Status</div>
          <div class="detail-field-value">
            <IssueBadge :status="issueStore.selectedIssue.status" />
          </div>
        </div>
        <div class="detail-field">
          <div class="detail-field-label">Priority</div>
          <div class="detail-field-value">
            <IssuePriority :priority="issueStore.selectedIssue.priority" />
          </div>
        </div>
        <div class="detail-field">
          <div class="detail-field-label">Assignee</div>
          <div class="detail-field-value" style="display: flex; align-items: center; gap: 6px">
            <AssigneeAvatar :assignee="issueStore.selectedIssue.assignee" size="md" />
            {{ issueStore.selectedIssue.assignee ?? 'Unassigned' }}
          </div>
        </div>
        <div class="detail-field">
          <div class="detail-field-label">Due Date</div>
          <div class="detail-field-value">
            {{ formatDueDateLong(issueStore.selectedIssue.dueDate) }}
          </div>
        </div>
        <div class="detail-field">
          <div class="detail-field-label">Type</div>
          <div class="detail-field-value">
            {{ issueStore.selectedIssue.type.charAt(0).toUpperCase() + issueStore.selectedIssue.type.slice(1) }}
          </div>
        </div>
        <div class="detail-field">
          <div class="detail-field-label">Description</div>
          <div class="detail-desc">
            {{ issueStore.selectedIssue.description || 'No description provided.' }}
          </div>
        </div>
      </div>

      <div class="detail-actions">
        <button class="btn btn-primary" @click="emit('edit')">✎ Edit</button>
        <button class="btn" @click="emit('subIssue')">+ Sub-issue</button>
        <button class="btn btn-danger" @click="emit('delete')">Delete</button>
      </div>
    </template>
  </div>
</template>
