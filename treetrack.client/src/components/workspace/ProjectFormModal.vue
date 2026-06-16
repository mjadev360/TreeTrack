<script setup lang="ts">
import { ref, watch } from 'vue'
import type { Project } from '@/types/project'

const props = defineProps<{
  open: boolean
  mode: 'create' | 'edit'
  project?: Project | null
}>()

const emit = defineEmits<{
  close: []
  save: [payload: { name: string; key: string }]
}>()

const name = ref('')
const key = ref('')
const saving = ref(false)
const localError = ref<string | null>(null)

const keyPattern = /^[A-Z0-9]{2,10}$/

watch(
  () => [props.open, props.project, props.mode] as const,
  ([isOpen, project, mode]) => {
    if (!isOpen) return

    localError.value = null
    if (mode === 'edit' && project) {
      name.value = project.name
      key.value = project.key
    } else {
      name.value = ''
      key.value = ''
    }
  },
  { immediate: true }
)

function handleKeyInput(event: Event) {
  const input = event.target as HTMLInputElement
  key.value = input.value.toUpperCase().replace(/[^A-Z0-9]/g, '')
}

function validate(): string | null {
  if (!name.value.trim()) {
    return 'Name is required'
  }
  if (!key.value.trim()) {
    return 'Key is required'
  }
  if (!keyPattern.test(key.value)) {
    return 'Key must be 2-10 uppercase letters or numbers'
  }
  return null
}

async function handleSave() {
  localError.value = validate()
  if (localError.value) return

  saving.value = true
  try {
    emit('save', {
      name: name.value.trim(),
      key: key.value.trim()
    })
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div v-if="open" class="modal-overlay" @click.self="emit('close')">
    <div class="modal">
      <div class="modal-header">
        {{ mode === 'create' ? 'New Project' : 'Edit Project' }}
      </div>
      <div class="modal-body">
        <div v-if="localError" class="form-error">{{ localError }}</div>

        <div class="form-field">
          <label class="form-label">Name</label>
          <input
            id="project-name"
            v-model="name"
            class="form-input"
            type="text"
            maxlength="200"
            placeholder="My Project"
          />
        </div>

        <div class="form-field">
          <label class="form-label">Key</label>
          <input
            id="project-key"
            :value="key"
            class="form-input"
            type="text"
            maxlength="10"
            placeholder="MP"
            @input="handleKeyInput"
          />
          <span class="form-hint">Used in issue IDs (e.g. MP-1). 2-10 uppercase letters or numbers.</span>
        </div>
      </div>
      <div class="modal-footer">
        <button class="btn" type="button" @click="emit('close')">Cancel</button>
        <button class="btn btn-primary" type="button" :disabled="saving" @click="handleSave">
          {{ saving ? 'Saving...' : mode === 'create' ? 'Create' : 'Save' }}
        </button>
      </div>
    </div>
  </div>
</template>
