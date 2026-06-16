import { defineStore } from 'pinia'
import { ref } from 'vue'
import { projectService } from '@/services/projectService'
import type { CreateProjectRequest, Project, UpdateProjectRequest } from '@/types/project'

export const useProjectStore = defineStore('projects', () => {
  const projects = ref<Project[]>([])
  const currentProject = ref<Project | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchAll() {
    loading.value = true
    error.value = null
    try {
      projects.value = await projectService.list()
    } catch (err) {
      error.value = extractErrorMessage(err, 'Failed to load projects')
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchById(id: number) {
    loading.value = true
    error.value = null
    try {
      currentProject.value = await projectService.getById(id)
      return currentProject.value
    } catch (err) {
      error.value = extractErrorMessage(err, 'Failed to load project')
      currentProject.value = null
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createProject(request: CreateProjectRequest) {
    error.value = null
    try {
      const created = await projectService.create(request)
      projects.value = [...projects.value, created].sort((a, b) =>
        a.name.localeCompare(b.name)
      )
      return created
    } catch (err) {
      error.value = extractErrorMessage(err, 'Failed to create project')
      throw err
    }
  }

  async function updateProject(id: number, request: UpdateProjectRequest) {
    error.value = null
    try {
      const updated = await projectService.update(id, request)
      projects.value = projects.value
        .map((p) => (p.id === id ? updated : p))
        .sort((a, b) => a.name.localeCompare(b.name))
      if (currentProject.value?.id === id) {
        currentProject.value = updated
      }
      return updated
    } catch (err) {
      error.value = extractErrorMessage(err, 'Failed to update project')
      throw err
    }
  }

  async function deleteProject(id: number) {
    error.value = null
    try {
      await projectService.delete(id)
      projects.value = projects.value.filter((p) => p.id !== id)
      if (currentProject.value?.id === id) {
        currentProject.value = null
      }
    } catch (err) {
      error.value = extractErrorMessage(err, 'Failed to delete project')
      throw err
    }
  }

  function clearError() {
    error.value = null
  }

  function clearCurrentProject() {
    currentProject.value = null
  }

  return {
    projects,
    currentProject,
    loading,
    error,
    fetchAll,
    fetchById,
    createProject,
    updateProject,
    deleteProject,
    clearError,
    clearCurrentProject
  }
})

function extractErrorMessage(err: unknown, fallback: string): string {
  if (err && typeof err === 'object' && 'response' in err) {
    const response = (err as { response?: { data?: { message?: string } } }).response
    if (response?.data?.message) {
      return response.data.message
    }
  }
  return err instanceof Error ? err.message : fallback
}
