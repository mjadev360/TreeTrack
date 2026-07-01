<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectStore } from '@/stores/projectStore'
import MainLayout from './layouts/MainLayout.vue'
import AuthLayout from './layouts/AuthLayout.vue'
import IssuesLayout from './layouts/IssuesLayout.vue'

const DEFAULT_TITLE = 'TreeTrack'

const route = useRoute()
const projectStore = useProjectStore()

watch(
  () => [route.name, projectStore.currentProject?.name] as const,
  ([routeName, projectName]) => {
    document.title =
      routeName === 'ProjectIssues' && projectName
        ? `TT - ${projectName}`
        : DEFAULT_TITLE
  },
  { immediate: true }
)

const layout = computed(() => route.meta.layout)

const LayoutComponent = computed(() => {
  if (layout.value === 'auth') return AuthLayout
  if (layout.value === 'issues') return IssuesLayout
  return MainLayout
})
</script>

<template>
  <component :is="LayoutComponent" />
</template>
