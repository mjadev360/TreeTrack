<script setup lang="ts">
import { computed } from 'vue'
import { useIssueStore } from '@/stores/issueStore'
import type { IssueTreeNode } from '@/types/issue'
import { TYPE_ICONS, formatDueDate } from '@/types/issue'
import IssueBadge from './IssueBadge.vue'
import IssuePriority from './IssuePriority.vue'
import AssigneeAvatar from './AssigneeAvatar.vue'

const props = defineProps<{
  node: IssueTreeNode
  depth: number
}>()

const issueStore = useIssueStore()

const hasChildren = computed(() => props.node.children.length > 0)
const isExpanded = computed(() => issueStore.expandedIds.has(props.node.id))
const isSelected = computed(() => issueStore.selectedId === props.node.id)
const typeIcon = computed(() => TYPE_ICONS[props.node.type] ?? TYPE_ICONS.task)

function handleRowClick() {
  issueStore.selectIssue(props.node.id)
}

function handleToggleClick(event: MouseEvent) {
  event.stopPropagation()
  if (hasChildren.value) {
    issueStore.toggleExpand(props.node.id)
  }
}
</script>

<template>
  <div class="node">
    <div class="node-row" :class="{ selected: isSelected }" @click="handleRowClick">
      <div class="node-indent" style="padding-left: 16px; display: flex; align-items: center">
        <div v-for="i in depth" :key="i" class="indent-guide"></div>
        <span
          class="node-toggle"
          :class="{ open: isExpanded, leaf: !hasChildren }"
          @click="handleToggleClick"
        >▶</span>
      </div>
      <div class="node-icon" :class="typeIcon.class">{{ typeIcon.icon }}</div>
      <div class="node-title-wrap">
        <span class="node-id">{{ node.key }}</span>
        <span class="node-title" :class="{ done: node.status === 'closed' }">{{ node.title }}</span>
      </div>
      <div class="col-status">
        <IssueBadge :status="node.status" />
      </div>
      <div class="col-priority">
        <IssuePriority :priority="node.priority" />
      </div>
      <div class="col-assignee">
        <AssigneeAvatar :assignee="node.assignee" />
      </div>
      <div class="col-date">{{ formatDueDate(node.dueDate) }}</div>
    </div>

    <div v-if="hasChildren && isExpanded">
      <IssueTreeNode
        v-for="child in node.children"
        :key="child.id"
        :node="child"
        :depth="depth + 1"
      />
    </div>
  </div>
</template>
