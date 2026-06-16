<script setup lang="ts">
import { useIssueStore } from '@/stores/issueStore'

const issueStore = useIssueStore()

const statusFilters = [
  { value: 'all', label: 'All' },
  { value: 'open', label: 'Open' },
  { value: 'in-progress', label: 'In Progress' },
  { value: 'review', label: 'Review' },
  { value: 'blocked', label: 'Blocked' },
  { value: 'closed', label: 'Closed' }
]

const priorityFilters = [
  { value: 'all', label: 'All' },
  { value: 'critical', label: 'Critical' },
  { value: 'high', label: 'High' },
  { value: 'medium', label: 'Medium' },
  { value: 'low', label: 'Low' }
]

const summaryItems = [
  { label: 'Epics', key: 'epic' as const, color: 'var(--purple)' },
  { label: 'Stories', key: 'story' as const, color: 'var(--blue)' },
  { label: 'Tasks', key: 'task' as const, color: 'var(--accent)' },
  { label: 'Bugs', key: 'bug' as const, color: 'var(--red)' },
  { label: 'Subtasks', key: 'subtask' as const, color: 'var(--text-muted)' }
]
</script>

<template>
  <div class="sidebar">
    <div class="sidebar-section">
      <div class="sidebar-label">Views</div>
      <div class="sidebar-item active">
        🌲 Tree View
        <span class="count">{{ issueStore.stats.total }}</span>
      </div>
      <div class="sidebar-item disabled">📋 Board</div>
      <div class="sidebar-item disabled">📊 Timeline</div>
      <div class="sidebar-item disabled">📈 Reports</div>
    </div>

    <div class="sidebar-section">
      <div class="sidebar-label">Filter by Status</div>
      <div class="filter-row">
        <div class="filter-chips">
          <button
            v-for="filter in statusFilters"
            :key="filter.value"
            class="chip"
            :class="{ active: issueStore.statusFilter === filter.value }"
            @click="issueStore.setStatusFilter(filter.value)"
          >
            {{ filter.label }}
          </button>
        </div>
      </div>
    </div>

    <div class="sidebar-section">
      <div class="sidebar-label">Filter by Priority</div>
      <div class="filter-row">
        <div class="filter-chips">
          <button
            v-for="filter in priorityFilters"
            :key="filter.value"
            class="chip"
            :class="{ active: issueStore.priorityFilter === filter.value }"
            @click="issueStore.setPriorityFilter(filter.value)"
          >
            {{ filter.label }}
          </button>
        </div>
      </div>
    </div>

    <div class="sidebar-section" style="flex: 1">
      <div class="sidebar-label">Summary</div>
      <div style="padding: 8px 14px; display: flex; flex-direction: column; gap: 6px">
        <div v-for="item in summaryItems" :key="item.key" class="summary-row">
          <span class="summary-dot" :style="{ background: item.color }"></span>
          <span style="color: var(--text-dim)">{{ item.label }}</span>
          <span style="margin-left: auto; color: var(--text-muted)">
            {{ issueStore.stats.byType[item.key] }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>
