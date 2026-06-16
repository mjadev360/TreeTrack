<script setup lang="ts">
import type { IssueStatus } from '@/types/issue'
import { STATUS_LABELS } from '@/types/issue'

defineProps<{
  status: IssueStatus
}>()

const badgeClass: Record<IssueStatus, string> = {
  open: 'badge-open',
  'in-progress': 'badge-progress',
  closed: 'badge-closed',
  blocked: 'badge-blocked',
  review: 'badge-review'
}
</script>

<template>
  <span class="badge" :class="badgeClass[status]">{{ STATUS_LABELS[status] }}</span>
</template>
