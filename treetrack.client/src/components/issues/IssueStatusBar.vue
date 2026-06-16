<script setup lang="ts">
import { useIssueStore } from '@/stores/issueStore'
import { useProjectStore } from '@/stores/projectStore'

const issueStore = useIssueStore()
const projectStore = useProjectStore()
</script>

<template>
  <div class="statusbar">
    <span>
      <div class="dot" style="background: var(--accent)"></div>
      <span>{{ issueStore.stats.open }} open</span>
    </span>
    <span>
      <div class="dot" style="background: var(--yellow)"></div>
      <span>{{ issueStore.stats.inProgress }} in progress</span>
    </span>
    <span>
      <div class="dot" style="background: var(--red)"></div>
      <span>{{ issueStore.stats.blocked }} blocked</span>
    </span>
    <span>
      <div class="dot" style="background: var(--text-muted)"></div>
      <span>{{ issueStore.stats.closed }} closed</span>
    </span>
    <span v-if="projectStore.currentProject" style="margin-left: auto; color: var(--text-muted)">
      {{ projectStore.currentProject.name }} · {{ projectStore.currentProject.key }}
    </span>
  </div>
</template>
