<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { useProjectStore } from '@/stores/projectStore'
import type { Project } from '@/types/project'
import '@/assets/issue-tracker.css'
import TopBar from '@/components/TopBar.vue'
import ProjectFormModal from '@/components/workspace/ProjectFormModal.vue'
import ShareProjectModal from '@/components/workspace/ShareProjectModal.vue'

const router = useRouter()
const authStore = useAuthStore()
const projectStore = useProjectStore()

const modalOpen = ref(false)
const modalMode = ref<'create' | 'edit'>('create')
const editingProject = ref<Project | null>(null)
const shareModalOpen = ref(false)
const sharingProject = ref<Project | null>(null)

const totalIssues = computed(() =>
  projectStore.projects.reduce((sum, p) => sum + p.issueCount, 0)
)

onMounted(() => {
  projectStore.fetchAll()
})

function openCreate() {
  projectStore.clearError()
  modalMode.value = 'create'
  editingProject.value = null
  modalOpen.value = true
}

function openEdit(project: Project) {
  projectStore.clearError()
  modalMode.value = 'edit'
  editingProject.value = project
  modalOpen.value = true
}

function openShare(project: Project) {
  sharingProject.value = project
  shareModalOpen.value = true
}

function openProject(project: Project) {
  router.push({ name: 'ProjectIssues', params: { projectId: project.id } })
}

async function handleSave(payload: { name: string; key: string }) {
  try {
    if (modalMode.value === 'create') {
      await projectStore.createProject(payload)
    } else if (editingProject.value) {
      await projectStore.updateProject(editingProject.value.id, payload)
    }
    modalOpen.value = false
  } catch {
    // error surfaced via store
  }
}

async function handleDelete(project: Project) {
  const confirmed = window.confirm(
    `Delete "${project.name}" and all its issues? This cannot be undone.`
  )
  if (!confirmed) return

  try {
    await projectStore.deleteProject(project.id)
  } catch {
    // error surfaced via store
  }
}

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'Login' })
}

function formatDate(dateStr: string) {
  return new Date(dateStr).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}
</script>

<template>
  <div class="issue-tracker">
    <TopBar>
      <template #breadcrumb>
        <strong>workspace</strong>
      </template>
      <template #actions>
        <button class="btn btn-primary" @click="openCreate">+ New Project</button>
        <button class="btn" @click="handleLogout">Logout</button>
      </template>
    </TopBar>

    <div class="layout">
      <div class="sidebar">
        <div class="sidebar-section">
          <div class="sidebar-label">Views</div>
          <div class="sidebar-item active">
            📁 Projects
            <span class="count">{{ projectStore.projects.length }}</span>
          </div>
        </div>

        <div class="sidebar-section">
          <div class="sidebar-label">Summary</div>
          <div class="summary-list">
            <div class="summary-row">
              <div class="summary-dot" style="background: var(--accent)"></div>
              <span>{{ projectStore.projects.length }} projects</span>
            </div>
            <div class="summary-row">
              <div class="summary-dot" style="background: var(--blue)"></div>
              <span>{{ totalIssues }} total issues</span>
            </div>
          </div>
        </div>
      </div>

      <div class="main workspace-main">
        <div v-if="projectStore.error" class="workspace-error">
          {{ projectStore.error }}
          <button class="btn btn-sm" @click="projectStore.clearError">Dismiss</button>
        </div>

        <div v-if="projectStore.loading" class="empty">
          <div class="empty-icon">⏳</div>
          <span>Loading projects...</span>
        </div>

        <div v-else-if="projectStore.projects.length === 0" class="empty">
          <div class="empty-icon">📁</div>
          <span>No projects yet</span>
          <span>Create your first project to start tracking issues.</span>
          <button class="btn btn-primary" style="margin-top: 8px" @click="openCreate">
            + Create Project
          </button>
        </div>

        <div v-else class="workspace-body">
          <div class="tree-header">
            <span class="col-title">Project</span>
            <span class="col-key">Key</span>
            <span class="col-issues">Issues</span>
            <span class="col-date">Created</span>
            <span class="col-actions">Actions</span>
          </div>

          <div class="project-list">
            <article
              v-for="project in projectStore.projects"
              :key="project.id"
              class="project-row"
              @click="openProject(project)"
            >
              <span class="col-title project-name">
                {{ project.name }}
                <span v-if="!project.isOwner" class="shared-badge">Shared</span>
              </span>
              <span class="col-key">
                <span class="project-key-badge">{{ project.key }}</span>
              </span>
              <span class="col-issues">{{ project.issueCount }}</span>
              <span class="col-date">{{ formatDate(project.createdAt) }}</span>
              <span class="col-actions" @click.stop>
                <button class="btn btn-sm" @click="openProject(project)">Open</button>
                <button v-if="project.isOwner" class="btn btn-sm" @click="openShare(project)">Share</button>
                <button v-if="project.isOwner" class="btn btn-sm" @click="openEdit(project)">Edit</button>
                <button v-if="project.isOwner" class="btn btn-sm btn-danger" @click="handleDelete(project)">Delete</button>
              </span>
            </article>
          </div>
        </div>
      </div>
    </div>

    <div class="statusbar">
      <span>
        <div class="dot" style="background: var(--accent)"></div>
        <span>{{ projectStore.projects.length }} projects</span>
      </span>
      <span>
        <div class="dot" style="background: var(--blue)"></div>
        <span>{{ totalIssues }} issues</span>
      </span>
      <span style="margin-left: auto; color: var(--text-muted)">workspace</span>
    </div>

    <ProjectFormModal
      :open="modalOpen"
      :mode="modalMode"
      :project="editingProject"
      @close="modalOpen = false"
      @save="handleSave"
    />

    <ShareProjectModal
      :open="shareModalOpen"
      :project="sharingProject"
      @close="shareModalOpen = false"
    />
  </div>
</template>

<style scoped>
.shared-badge {
  display: inline-block;
  margin-left: 8px;
  padding: 1px 6px;
  font-size: 10px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  color: var(--text-muted);
  border: 1px solid var(--border);
  border-radius: 4px;
  vertical-align: middle;
}
</style>
