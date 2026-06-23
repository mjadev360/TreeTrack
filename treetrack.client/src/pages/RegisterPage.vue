<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { inviteService } from '@/services/inviteService'
import type { RegisterRequest } from '@/services/authService'
import '@/assets/issue-tracker.css'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const submitted = ref(false)
const inviteToken = ref<string | null>(null)
const inviteEmailLocked = ref(false)

const passwordsMatch = computed(() => password.value === confirmPassword.value)

const passwordValid = computed(
  () =>
    password.value.length >= 8 &&
    /[A-Z]/.test(password.value) &&
    /[a-z]/.test(password.value) &&
    /\d/.test(password.value) &&
    /[^A-Za-z0-9]/.test(password.value)
)

const formValid = computed(
  () =>
    email.value.trim() !== '' &&
    passwordValid.value &&
    confirmPassword.value.trim() !== '' &&
    passwordsMatch.value
)

onMounted(async () => {
  const token = route.query.invite as string | undefined
  if (!token) return

  inviteToken.value = token
  try {
    const preview = await inviteService.getPreview(token)
    email.value = preview.email
    inviteEmailLocked.value = true
  } catch {
    // invite preview unavailable; user can still register normally
  }
})

const handleSubmit = async () => {
  if (!formValid.value) return

  submitted.value = true
  try {
    const registerData: RegisterRequest = {
      email: email.value,
      password: password.value,
      confirmPassword: confirmPassword.value
    }

    await authStore.register(registerData)
    if (!authStore.isAuthenticated) return

    if (inviteToken.value) {
      router.push({ name: 'Invite', params: { token: inviteToken.value } })
    } else {
      router.push({ name: 'Workspace' })
    }
  } catch (error) {
    console.error('Registration error:', error)
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
      <h1 class="auth-title">Create Account</h1>

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
            :readonly="inviteEmailLocked"
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
          <div v-if="submitted || password" class="password-requirements">
            <div :class="{ valid: password.length >= 8, invalid: password.length > 0 && password.length < 8 }">
              ✓ At least 8 characters
            </div>
            <div :class="{ valid: /[A-Z]/.test(password), invalid: password.length > 0 && !/[A-Z]/.test(password) }">
              ✓ One uppercase letter
            </div>
            <div :class="{ valid: /[a-z]/.test(password), invalid: password.length > 0 && !/[a-z]/.test(password) }">
              ✓ One lowercase letter
            </div>
            <div :class="{ valid: /\d/.test(password), invalid: password.length > 0 && !/\d/.test(password) }">
              ✓ One number
            </div>
            <div :class="{ valid: /[^A-Za-z0-9]/.test(password), invalid: password.length > 0 && !/[^A-Za-z0-9]/.test(password) }">
              ✓ One special character
            </div>
          </div>
        </div>

        <div class="form-field">
          <label class="form-label" for="confirmPassword">Confirm Password</label>
          <input
            id="confirmPassword"
            v-model="confirmPassword"
            class="form-input"
            type="password"
            placeholder="Confirm your password"
            required
            :disabled="authStore.isLoading"
          />
          <span v-if="confirmPassword && !passwordsMatch" class="form-hint error">
            Passwords do not match
          </span>
          <span v-else-if="confirmPassword && passwordsMatch" class="form-hint valid">
            ✓ Passwords match
          </span>
        </div>

        <button
          type="submit"
          class="btn btn-primary auth-submit"
          :disabled="!formValid || authStore.isLoading"
        >
          {{ authStore.isLoading ? 'Creating account...' : 'Register' }}
        </button>
      </form>

      <div class="auth-footer">
        <p>
          Already have an account?
          <RouterLink :to="inviteToken ? `/login?invite=${inviteToken}` : '/login'">Login here</RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>
