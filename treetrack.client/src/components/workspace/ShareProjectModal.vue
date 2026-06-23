<script setup lang="ts">
import { ref, watch } from 'vue'
import {
  buildInviteUrl,
  inviteService,
  type Invite,
  type ProjectCollaborators
} from '@/services/inviteService'
import type { Project } from '@/types/project'

const props = defineProps<{
  open: boolean
  project: Project | null
}>()

const emit = defineEmits<{
  close: []
}>()

const email = ref('')
const loading = ref(false)
const saving = ref(false)
const localError = ref<string | null>(null)
const collaborators = ref<ProjectCollaborators | null>(null)
const createdInvite = ref<Invite | null>(null)
const copied = ref(false)

watch(
  () => [props.open, props.project] as const,
  async ([isOpen, project]) => {
    if (!isOpen || !project) return

    email.value = ''
    localError.value = null
    createdInvite.value = null
    copied.value = false
    await loadCollaborators(project.id)
  },
  { immediate: true }
)

async function loadCollaborators(projectId: number) {
  loading.value = true
  localError.value = null
  try {
    collaborators.value = await inviteService.listCollaborators(projectId)
  } catch (err) {
    localError.value = extractError(err, 'Failed to load collaborators')
  } finally {
    loading.value = false
  }
}

async function handleCreateInvite() {
  if (!props.project || !email.value.trim()) return

  saving.value = true
  localError.value = null
  createdInvite.value = null
  copied.value = false

  try {
    const invite = await inviteService.createInvite(props.project.id, email.value.trim())
    createdInvite.value = invite
    email.value = ''
    await loadCollaborators(props.project.id)
  } catch (err) {
    localError.value = extractError(err, 'Failed to create invite')
  } finally {
    saving.value = false
  }
}

async function handleRevoke(inviteId: number) {
  if (!props.project) return

  try {
    await inviteService.revokeInvite(props.project.id, inviteId)
    await loadCollaborators(props.project.id)
  } catch (err) {
    localError.value = extractError(err, 'Failed to revoke invite')
  }
}

async function handleRemoveMember(userId: string) {
  if (!props.project) return

  const confirmed = window.confirm('Remove this member from the project?')
  if (!confirmed) return

  try {
    await inviteService.removeMember(props.project.id, userId)
    await loadCollaborators(props.project.id)
  } catch (err) {
    localError.value = extractError(err, 'Failed to remove member')
  }
}

async function copyLink(token: string) {
  try {
    await navigator.clipboard.writeText(buildInviteUrl(token))
    copied.value = true
    setTimeout(() => { copied.value = false }, 2000)
  } catch {
    localError.value = 'Failed to copy link'
  }
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
  <div v-if="open" class="modal-overlay" @click.self="emit('close')">
    <div class="modal share-modal">
      <div class="modal-header">
        Share "{{ project?.name }}"
      </div>
      <div class="modal-body">
        <div v-if="localError" class="form-error">{{ localError }}</div>

        <div class="form-field">
          <label class="form-label">Invite by email</label>
          <div class="share-invite-row">
            <input
              v-model="email"
              class="form-input"
              type="email"
              placeholder="colleague@example.com"
              :disabled="saving"
            />
            <button
              class="btn btn-primary"
              type="button"
              :disabled="saving || !email.trim()"
              @click="handleCreateInvite"
            >
              {{ saving ? 'Creating...' : 'Create invite' }}
            </button>
          </div>
          <span class="form-hint">An invite link will be generated for you to copy and send.</span>
        </div>

        <div v-if="createdInvite" class="share-link-box">
          <label class="form-label">Invite link for {{ createdInvite.email }}</label>
          <div class="share-invite-row">
            <input
              class="form-input"
              type="text"
              readonly
              :value="buildInviteUrl(createdInvite.token)"
            />
            <button class="btn" type="button" @click="copyLink(createdInvite.token)">
              {{ copied ? 'Copied!' : 'Copy link' }}
            </button>
          </div>
        </div>

        <div v-if="loading" class="share-loading">Loading collaborators...</div>

        <template v-else-if="collaborators">
          <div v-if="collaborators.members.length > 0" class="share-section">
            <div class="share-section-title">Members</div>
            <div
              v-for="member in collaborators.members"
              :key="member.userId"
              class="share-list-item"
            >
              <span>{{ member.email }}</span>
              <button class="btn btn-sm btn-danger" @click="handleRemoveMember(member.userId)">
                Remove
              </button>
            </div>
          </div>

          <div v-if="collaborators.pendingInvites.length > 0" class="share-section">
            <div class="share-section-title">Pending invites</div>
            <div
              v-for="invite in collaborators.pendingInvites"
              :key="invite.id"
              class="share-list-item"
            >
              <span>{{ invite.email }}</span>
              <span class="share-list-actions">
                <button class="btn btn-sm" @click="copyLink(invite.token)">Copy link</button>
                <button class="btn btn-sm btn-danger" @click="handleRevoke(invite.id)">
                  Revoke
                </button>
              </span>
            </div>
          </div>

          <div
            v-if="collaborators.members.length === 0 && collaborators.pendingInvites.length === 0"
            class="share-empty"
          >
            No collaborators yet.
          </div>
        </template>
      </div>
      <div class="modal-footer">
        <button class="btn" type="button" @click="emit('close')">Close</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.share-modal {
  max-width: 520px;
}

.share-invite-row {
  display: flex;
  gap: 8px;
}

.share-invite-row .form-input {
  flex: 1;
}

.share-link-box {
  margin-top: 16px;
  padding: 12px;
  background: var(--bg2);
  border: 1px solid var(--border);
  border-radius: 6px;
}

.share-section {
  margin-top: 20px;
}

.share-section-title {
  font-size: 12px;
  font-weight: 600;
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 8px;
}

.share-list-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid var(--border);
  gap: 8px;
}

.share-list-actions {
  display: flex;
  gap: 6px;
  flex-shrink: 0;
}

.share-loading,
.share-empty {
  margin-top: 16px;
  color: var(--text-muted);
  font-size: 13px;
}
</style>
