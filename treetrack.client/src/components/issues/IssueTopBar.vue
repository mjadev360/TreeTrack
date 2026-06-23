<script setup lang="ts">
import { ref } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import { useIssueStore } from '@/stores/issueStore'
import { useProjectStore } from '@/stores/projectStore'
import { useRouter } from 'vue-router'
import ShareProjectModal from '@/components/workspace/ShareProjectModal.vue'

const emit = defineEmits<{
  newIssue: []
}>()

const issueStore = useIssueStore()
const projectStore = useProjectStore()
const authStore = useAuthStore()
const router = useRouter()
const shareModalOpen = ref(false)

const handleLogout = async () => {
  await authStore.logout()
  router.push({ name: 'Login' })
}
</script>

<template>
  <div class="topbar">
    <div class="logo">Tree<span>Track</span></div>
    <div class="topbar-sep"></div>
    <div class="breadcrumb">
      <router-link to="/workspace" class="breadcrumb-link">workspace</router-link>
      <span style="color: var(--border2)">/</span>
      <strong>{{ projectStore.currentProject?.name ?? '...' }}</strong>
    </div>
    <div class="topbar-actions">
      <button class="btn" @click="issueStore.collapseAll()">⊟ Collapse All</button>
      <button class="btn" @click="issueStore.expandAll()">⊞ Expand All</button>
      <button
        v-if="projectStore.currentProject?.isOwner"
        class="btn"
        @click="shareModalOpen = true"
      >
        Share
      </button>
      <button class="btn btn-primary" @click="emit('newIssue')">+ New Issue</button>
      <button class="btn" @click="handleLogout">Logout</button>
    </div>
  </div>

  <ShareProjectModal
    :open="shareModalOpen"
    :project="projectStore.currentProject"
    @close="shareModalOpen = false"
  />
</template>

<style scoped>
.breadcrumb-link {
  color: var(--text-muted);
  text-decoration: none;
}

.breadcrumb-link:hover {
  color: var(--text);
}
</style>
