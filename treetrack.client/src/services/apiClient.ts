import axios, { type AxiosInstance } from 'axios'

export const API_BASE_URL =
  import.meta.env.VITE_API_URL ??
  (import.meta.env.DEV ? '/api' : 'https://localhost:7230/api')

const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true
})

export default apiClient
