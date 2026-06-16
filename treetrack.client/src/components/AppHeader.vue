<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

const handleLogout = async () => {
  await authStore.logout()
  router.push({ name: 'Login' })
}
</script>

<template>
  <header class="header">
    <div class="header-content">
      <div class="header-left">
        <h1 class="app-title">
          <router-link to="/workspace" class="app-title-link">TreeTrack</router-link>
        </h1>
        <nav class="header-nav">
          <router-link to="/workspace" class="nav-link">Workspace</router-link>
        </nav>
      </div>
      <div class="header-actions">
        <span class="user-name">{{ authStore.user?.email }}</span>
        <button class="logout-btn" @click="handleLogout">Logout</button>
      </div>
    </div>
  </header>
</template>

<style scoped>
.header {
  width: 100%;
  flex-shrink: 0;
  background-color: #2c5530;
  color: #ecf0f1;
  padding: 15px 20px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  z-index: 100;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 0 auto;
}

.app-title {
  margin: 0;
  font-size: 32px;
  font-weight: 600;
}

.app-title-link {
  color: #a8e6a3;
  text-decoration: none;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 24px;
}

.header-nav {
  display: flex;
  gap: 16px;
}

.nav-link {
  color: #bdc3c7;
  text-decoration: none;
  font-size: 14px;
}

.nav-link:hover,
.nav-link.router-link-active {
  color: #a8e6a3;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 20px;
}

.user-name {
  font-size: 14px;
  color: #bdc3c7;
  text-decoration: none;
}

.logout-btn {
  padding: 8px 16px;
  background-color: #e74c3c;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.logout-btn:hover {
  background-color: #c0392b;
}
</style>
