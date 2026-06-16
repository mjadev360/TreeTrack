import axios, { type AxiosInstance } from 'axios'

// Defaults to same-origin /api (Vite proxy in dev, Railway in prod).
// Set VITE_API_URL to a full https URL only when the API is on a separate host.
export const API_BASE_URL = import.meta.env.VITE_API_URL ?? '/api'

const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true
})

export default apiClient
