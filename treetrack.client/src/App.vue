<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import MainLayout from './layouts/MainLayout.vue'
import AuthLayout from './layouts/AuthLayout.vue'
import IssuesLayout from './layouts/IssuesLayout.vue'

const route = useRoute()

const layout = computed(() => route.meta.layout)

const LayoutComponent = computed(() => {
  if (layout.value === 'auth') return AuthLayout
  if (layout.value === 'issues') return IssuesLayout
  return MainLayout
})
</script>

<template>
  <component :is="LayoutComponent" />
</template>
