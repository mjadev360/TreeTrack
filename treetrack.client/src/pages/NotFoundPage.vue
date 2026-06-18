<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import '@/assets/issue-tracker.css'

const authStore = useAuthStore()
const router = useRouter()

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'Login' })
}
</script>

<template>
  <div class="issue-tracker not-found-page">
    <div class="topbar">
      <div class="logo">Tree<span>Track</span></div>
      <div class="topbar-sep"></div>
      <div class="breadcrumb">
        <strong>404</strong>
      </div>
      <div class="topbar-actions">
        <RouterLink to="/workspace" class="btn btn-primary">Go to workspace</RouterLink>
        <button v-if="authStore.isAuthenticated" class="btn" @click="handleLogout">
          Logout
        </button>
      </div>
    </div>

    <div class="not-found-body">
      <h1 class="not-found-code">404</h1>
      <h2 class="not-found-title">Page Not Found</h2>
      <p class="not-found-message">Sorry, the page you're looking for doesn't exist.</p>
    </div>
  </div>
</template>

<style scoped>
.not-found-body {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: 24px;
}

.not-found-code {
  font-family: 'Syne', sans-serif;
  font-size: 72px;
  font-weight: 800;
  color: var(--accent);
  margin: 0 0 8px;
  letter-spacing: -2px;
  line-height: 1;
}

.not-found-title {
  font-family: 'Syne', sans-serif;
  font-size: 14px;
  font-weight: 600;
  color: var(--text);
  margin: 0 0 12px;
}

.not-found-message {
  font-size: 12px;
  color: var(--text-dim);
  margin: 0;
}

.topbar-actions .btn {
  display: inline-block;
  text-decoration: none;
}
</style>
