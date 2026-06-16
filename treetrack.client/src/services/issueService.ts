import apiClient from './apiClient'
import type {
  CreateIssueRequest,
  IssueDetail,
  IssueTreeNode,
  UpdateIssueRequest
} from '@/types/issue'

export const issueService = {
  async getTree(projectId: number): Promise<IssueTreeNode[]> {
    const { data } = await apiClient.get<IssueTreeNode[]>(`/projects/${projectId}/issues/tree`)
    return data
  },

  async getById(projectId: number, id: number): Promise<IssueDetail> {
    const { data } = await apiClient.get<IssueDetail>(`/projects/${projectId}/issues/${id}`)
    return data
  },

  async create(projectId: number, request: CreateIssueRequest): Promise<IssueDetail> {
    const { data } = await apiClient.post<IssueDetail>(`/projects/${projectId}/issues`, request)
    return data
  },

  async update(projectId: number, id: number, request: UpdateIssueRequest): Promise<IssueDetail> {
    const { data } = await apiClient.put<IssueDetail>(
      `/projects/${projectId}/issues/${id}`,
      request
    )
    return data
  },

  async delete(projectId: number, id: number): Promise<void> {
    await apiClient.delete(`/projects/${projectId}/issues/${id}`)
  }
}
