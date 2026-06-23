<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { inviteService } from '@/services/inviteService'
import { useAuthStore } from '@/stores/authStore'
import '@/assets/issue-tracker.css'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const token = computed(() => route.params.token as string)
const loading = ref(true)
const accepting = ref(false)
const error = ref<string | null>(null)
const projectName = ref('')
const inviteEmail = ref('')
const isExpired = ref(false)
const isAccepted = ref(false)

const emailMatches = computed(() => {
  if (!authStore.user?.email || !inviteEmail.value) return false
  return authStore.user.email.toLowerCase() === inviteEmail.value.toLowerCase()
})

onMounted(async () => {
  try {
    const preview = await inviteService.getPreview(token.value)
    projectName.value = preview.projectName
    inviteEmail.value = preview.email
    isExpired.value = preview.isExpired
    isAccepted.value = preview.isAccepted
  } catch (err) {
    error.value = extractError(err, 'Invite not found')
  } finally {
    loading.value = false
  }
})

async function handleAccept() {
  accepting.value = true
  error.value = null
  try {
    const result = await inviteService.accept(token.value)
    router.push({ name: 'ProjectIssues', params: { projectId: result.projectId } })
  } catch (err) {
    error.value = extractError(err, 'Failed to accept invite')
  } finally {
    accepting.value = false
  }
}

function goToLogin() {
  router.push({ name: 'Login', query: { invite: token.value } })
}

function goToRegister() {
  router.push({ name: 'Register', query: { invite: token.value } })
}

function extractError(err: unknown, fallback: string): string {
  if (err && typeof err === 'object' && 'response' in err) {
    const response = (err as { response?: { data?: { message?: string } } }).response
    if (response?.data?.message) return response.data.message
  }
  return err instanceof Error ? err.message : fallback
}
</script>

<template>
  <div class="issue-tracker auth-page">
    <div class="auth-panel">
      <div class="auth-logo">TreeTrack</div>
      <h1 class="auth-title">Project Invite</h1>

      <div v-if="loading" class="invite-status">Loading invite...</div>

      <template v-else-if="error && !projectName">
        <div class="form-error">{{ error }}</div>
        <RouterLink to="/login" class="btn auth-submit" style="display: block; text-align: center; margin-top: 16px">
          Go to login
        </RouterLink>
      </template>

      <template v-else>
        <p class="invite-description">
          You've been invited to collaborate on
          <strong>{{ projectName }}</strong>.
        </p>
        <p class="invite-email">Invited as: {{ inviteEmail }}</p>

        <div v-if="isAccepted" class="form-error">
          This invite has already been accepted.
        </div>
        <div v-else-if="isExpired" class="form-error">
          This invite has expired. Ask the project owner for a new link.
        </div>

        <div v-if="error" class="form-error">{{ error }}</div>

        <template v-if="!isExpired && !isAccepted">
          <template v-if="authStore.isAuthenticated">
            <template v-if="emailMatches">
              <button
                class="btn btn-primary auth-submit"
                :disabled="accepting"
                @click="handleAccept"
              >
                {{ accepting ? 'Accepting...' : 'Accept invite' }}
              </button>
            </template>
            <template v-else>
              <p class="invite-hint">
                You're logged in as {{ authStore.user?.email }}. Log in with
                {{ inviteEmail }} to accept this invite.
              </p>
              <button class="btn btn-primary auth-submit" @click="goToLogin">
                Log in as {{ inviteEmail }}
              </button>
            </template>
          </template>
          <template v-else>
            <p class="invite-hint">
              Log in or create an account with {{ inviteEmail }} to join this project.
            </p>
            <button class="btn btn-primary auth-submit" @click="goToLogin">Log in</button>
            <button class="btn auth-submit" style="margin-top: 8px" @click="goToRegister">
              Create account
            </button>
          </template>
        </template>
      </template>
    </div>
  </div>
</template>

<style scoped>
.invite-description {
  color: var(--text-muted);
  margin-bottom: 8px;
  line-height: 1.5;
}

.invite-email {
  font-size: 13px;
  color: var(--text-muted);
  margin-bottom: 20px;
}

.invite-hint {
  font-size: 13px;
  color: var(--text-muted);
  margin-bottom: 16px;
  line-height: 1.5;
}

.invite-status {
  color: var(--text-muted);
  text-align: center;
}
</style>
