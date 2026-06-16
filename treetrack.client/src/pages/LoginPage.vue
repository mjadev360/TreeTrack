<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import type { LoginRequest } from '@/services/authService'
import '@/assets/issue-tracker.css'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const rememberMe = ref(false)
const submitted = ref(false)

const formValid = computed(
  () => email.value.trim() !== '' && password.value.trim() !== ''
)

const handleSubmit = async () => {
  if (!formValid.value) return

  submitted.value = true
  try {
    const loginData: LoginRequest = {
      email: email.value,
      password: password.value
    }

    await authStore.login(loginData)

    if (!authStore.isAuthenticated) return

    const redirect = route.query.redirect as string | undefined
    router.push(redirect || { name: 'Workspace' })
  } catch (error) {
    console.error('Login error:', error)
  }
}

const handleClearError = () => {
  authStore.clearError()
}
</script>

<template>
  <div class="issue-tracker auth-page">
    <div class="auth-panel">
      <div class="auth-logo">TreeTrack</div>
      <h1 class="auth-title">Login</h1>

      <form class="auth-form" @submit.prevent="handleSubmit">
        <div v-if="authStore.error" class="form-error">
          <button type="button" class="auth-close-btn" @click="handleClearError">×</button>
          {{ authStore.error }}
        </div>

        <div class="form-field">
          <label class="form-label" for="email">Email Address</label>
          <input
            id="email"
            v-model="email"
            class="form-input"
            type="email"
            placeholder="Enter your email"
            required
            :disabled="authStore.isLoading"
          />
        </div>

        <div class="form-field">
          <label class="form-label" for="password">Password</label>
          <input
            id="password"
            v-model="password"
            class="form-input"
            type="password"
            placeholder="Enter your password"
            required
            :disabled="authStore.isLoading"
          />
        </div>

        <div class="auth-checkbox">
          <input
            id="rememberMe"
            v-model="rememberMe"
            type="checkbox"
            :disabled="authStore.isLoading"
          />
          <label for="rememberMe">Remember me</label>
        </div>

        <button
          type="submit"
          class="btn btn-primary auth-submit"
          :disabled="!formValid || authStore.isLoading"
        >
          {{ authStore.isLoading ? 'Logging in...' : 'Login' }}
        </button>
      </form>

      <div class="auth-footer">
        <p>Don't have an account? <RouterLink to="/register">Register here</RouterLink></p>
      </div>
    </div>
  </div>
</template>
