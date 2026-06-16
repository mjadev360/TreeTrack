<script setup lang="ts">
import { computed } from 'vue'
import { AVATARS } from '@/types/issue'

const props = defineProps<{
  assignee: string | null
  size?: 'sm' | 'md'
}>()

const avatar = computed(() => {
  if (!props.assignee) {
    return { color: '#aaa', bg: '#333', label: '?' }
  }
  return AVATARS[props.assignee] ?? {
    color: '#aaa',
    bg: '#333',
    label: props.assignee.slice(0, 2).toUpperCase()
  }
})

const sizeStyle = computed(() => {
  if (props.size === 'md') {
    return { width: '22px', height: '22px', fontSize: '9px' }
  }
  return {}
})
</script>

<template>
  <span
    class="avatar"
    :style="{ background: avatar.bg, color: avatar.color, ...sizeStyle }"
  >
    {{ avatar.label }}
  </span>
</template>
