<script setup lang="ts">
import { useIssueStore } from '@/stores/issueStore'
import IssueTreeNode from './IssueTreeNode.vue'

const issueStore = useIssueStore()
</script>

<template>
  <div class="main">
    <div class="tree-header">
      <div class="col-title" style="padding-left: 16px">Title</div>
      <div class="col-status">Status</div>
      <div class="col-priority">Priority</div>
      <div class="col-assignee">Assignee</div>
      <div class="col-date">Due</div>
    </div>

    <div class="tree-body">
      <div v-if="issueStore.loading" class="empty">
        <div class="empty-icon">🌲</div>
        Loading issues...
      </div>
      <div v-else-if="issueStore.error" class="empty">
        <div class="empty-icon">⚠</div>
        {{ issueStore.error }}
      </div>
      <div v-else-if="issueStore.filteredTree.length === 0" class="empty">
        <div class="empty-icon">🔍</div>
        No issues match your filters.
      </div>
      <template v-else>
        <IssueTreeNode
          v-for="node in issueStore.filteredTree"
          :key="node.id"
          :node="node"
          :depth="0"
        />
      </template>
    </div>
  </div>
</template>
