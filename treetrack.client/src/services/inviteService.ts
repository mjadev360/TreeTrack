import apiClient from './apiClient'

export interface InvitePreview {
  projectName: string
  email: string
  isExpired: boolean
  isAccepted: boolean
}

export interface Invite {
  id: number
  email: string
  token: string
  createdAt: string
  expiresAt: string
}

export interface ProjectMember {
  userId: string
  email: string
  joinedAt: string
}

export interface ProjectCollaborators {
  members: ProjectMember[]
  pendingInvites: Invite[]
}

export const inviteService = {
  async getPreview(token: string): Promise<InvitePreview> {
    const { data } = await apiClient.get<InvitePreview>(`/invites/${token}`)
    return data
  },

  async accept(token: string): Promise<{ projectId: number }> {
    const { data } = await apiClient.post<{ projectId: number }>(`/invites/${token}/accept`)
    return data
  },

  async listCollaborators(projectId: number): Promise<ProjectCollaborators> {
    const { data } = await apiClient.get<ProjectCollaborators>(`/projects/${projectId}/invites`)
    return data
  },

  async createInvite(projectId: number, email: string): Promise<Invite> {
    const { data } = await apiClient.post<Invite>(`/projects/${projectId}/invites`, { email })
    return data
  },

  async revokeInvite(projectId: number, inviteId: number): Promise<void> {
    await apiClient.delete(`/projects/${projectId}/invites/${inviteId}`)
  },

  async removeMember(projectId: number, userId: string): Promise<void> {
    await apiClient.delete(`/projects/${projectId}/members/${userId}`)
  }
}

export function buildInviteUrl(token: string): string {
  return `${window.location.origin}/invite/${token}`
}
