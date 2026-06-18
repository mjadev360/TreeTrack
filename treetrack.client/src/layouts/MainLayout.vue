<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import AppHeader from '@/components/AppHeader.vue'

const authStore = useAuthStore()
</script>

<template>
  <div v-if="!authStore.isInitialized" class="loading-container">
    <div class="spinner"></div>
    <p>Loading...</p>
  </div>
  <div v-else class="app-wrapper">
    <AppHeader />
    <main class="main-content">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.loading-container {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 100%;
  background: #0d0f12;
  color: #e2e8f0;
}

.spinner {
  border: 4px solid rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  border-top: 4px solid #4fffb0;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

p {
  font-size: 14px;
  margin: 0;
  font-family: 'JetBrains Mono', monospace;
}

.app-wrapper {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.main-content {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  background: #f5f5f5;
}
</style>
