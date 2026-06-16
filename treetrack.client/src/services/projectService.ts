import apiClient from './apiClient'
import type { CreateProjectRequest, Project, UpdateProjectRequest } from '@/types/project'

export const projectService = {
  async list(): Promise<Project[]> {
    const { data } = await apiClient.get<Project[]>('/projects')
    return data
  },

  async getById(id: number): Promise<Project> {
    const { data } = await apiClient.get<Project>(`/projects/${id}`)
    return data
  },

  async create(request: CreateProjectRequest): Promise<Project> {
    const { data } = await apiClient.post<Project>('/projects', request)
    return data
  },

  async update(id: number, request: UpdateProjectRequest): Promise<Project> {
    const { data } = await apiClient.put<Project>(`/projects/${id}`, request)
    return data
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/projects/${id}`)
  }
}
