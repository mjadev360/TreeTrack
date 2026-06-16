<script setup lang="ts">
import { onMounted, ref } from 'vue'

interface WeatherForecast {
  date: string
  temperatureC: number
  temperatureF: number
  summary: string | null
}

const forecasts = ref<WeatherForecast[]>([])
const error = ref<string | null>(null)
const loading = ref(true)

onMounted(async () => {
  try {
    const response = await fetch('/weatherforecast')
    if (!response.ok) {
      throw new Error(`API request failed (${response.status})`)
    }

    forecasts.value = await response.json()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to load weather forecast'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <main class="home">
    <section class="hero">
      <h1>TreeTrack</h1>
      <p>Vue frontend and .NET API starter, modeled after FieldDeskPro.</p>
    </section>

    <section class="panel">
      <h2>Sample API: Weather Forecast</h2>
      <p v-if="loading">Loading forecast from the API...</p>
      <p v-else-if="error" class="error">{{ error }}</p>
      <ul v-else class="forecast-list">
        <li v-for="forecast in forecasts" :key="forecast.date">
          <strong>{{ forecast.date }}</strong>
          {{ forecast.temperatureC }}°C / {{ forecast.temperatureF }}°F
          <span v-if="forecast.summary"> — {{ forecast.summary }}</span>
        </li>
      </ul>
    </section>
  </main>
</template>

<style scoped>
.home {
  width: 100%;
  height: 100%;
  padding: 2rem;
  box-sizing: border-box;
}

.hero h1 {
  margin: 0 0 0.5rem;
  color: #2c5530;
}

.hero p {
  margin: 0;
  color: #555;
}

.panel {
  margin-top: 2rem;
  padding: 1.5rem;
  border: 1px solid var(--color-border);
  border-radius: 8px;
  background: var(--color-background-soft);
}

.panel h2 {
  margin: 0 0 1rem;
  font-size: 1.25rem;
}

.forecast-list {
  margin: 0;
  padding-left: 1.25rem;
}

.forecast-list li {
  margin: 0.5rem 0;
}

.error {
  color: #b00020;
}
</style>
