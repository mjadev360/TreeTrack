<script setup lang="ts">
import type { IssuePriority } from '@/types/issue'

defineProps<{
  priority: IssuePriority
}>()

const priorityClass: Record<IssuePriority, string> = {
  critical: 'p-critical',
  high: 'p-high',
  medium: 'p-medium',
  low: 'p-low'
}
</script>

<template>
  <span class="priority" :class="priorityClass[priority]">
    <span class="priority-dot"></span>
    {{ priority }}
  </span>
</template>
