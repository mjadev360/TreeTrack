import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authService, type RegisterRequest, type LoginRequest } from '@/services/authService'
import type { AxiosError } from 'axios'

export interface User {
  id: string
  email: string
  userName: string
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const isInitialized = ref(false)

  const isAuthenticated = computed(() => user.value !== null)

  const getErrorMessage = (err: unknown): string => {
    if (err instanceof Error) {
      const axiosError = err as AxiosError<{ message: string }>
      return axiosError.response?.data?.message || err.message || 'An error occurred'
    }
    return 'An error occurred'
  }

  const establishSession = async (data: LoginRequest): Promise<boolean> => {
    const response = await authService.login(data)
    if (!response.success) {
      error.value = response.message
      return false
    }

    const session = await authService.getCurrentUser()
    if (session.success && session.user) {
      user.value = session.user
      return true
    }

    user.value = null
    error.value = 'Login succeeded but session could not be established.'
    return false
  }

  const login = async (data: LoginRequest) => {
    isLoading.value = true
    error.value = null
    try {
      await establishSession(data)
    } catch (err) {
      const message = getErrorMessage(err)
      error.value = message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const register = async (data: RegisterRequest) => {
    isLoading.value = true
    error.value = null
    try {
      const response = await authService.register(data)
      if (!response.success) {
        error.value = response.message
        return response
      }

      const signedIn = await establishSession({
        email: data.email,
        password: data.password
      })

      if (!signedIn) {
        error.value = error.value ?? 'Account created. Please log in.'
      }

      return response
    } catch (err) {
      const message = getErrorMessage(err)
      error.value = message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const logout = async () => {
    isLoading.value = true
    error.value = null
    try {
      await authService.logout()
      user.value = null
    } catch (err) {
      const message = getErrorMessage(err)
      error.value = message
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const checkAuth = async () => {
    try {
      const response = await authService.getCurrentUser()
      if (response.success && response.user) {
        user.value = response.user
      } else {
        user.value = null
      }
    } catch {
      user.value = null
    } finally {
      isInitialized.value = true
    }
  }

  const clearError = () => {
    error.value = null
  }

  return {
    user,
    isLoading,
    error,
    isInitialized,
    isAuthenticated,
    register,
    login,
    logout,
    checkAuth,
    clearError
  }
})
