export interface Project {
  id: number
  name: string
  key: string
  createdAt: string
  issueCount: number
  isOwner: boolean
}

export interface CreateProjectRequest {
  name: string
  key: string
}

export interface UpdateProjectRequest {
  name: string
  key: string
}
