<script setup lang="ts">
import { useIssueStore } from '@/stores/issueStore'

const issueStore = useIssueStore()
</script>

<template>
  <div class="search-bar">
    <span style="color: var(--text-muted); font-size: 12px">⌕</span>
    <input
      class="search-input"
      placeholder="Search issues... (title, id, assignee)"
      :value="issueStore.searchQuery"
      @input="issueStore.setSearchQuery(($event.target as HTMLInputElement).value)"
    />
  </div>
</template>
