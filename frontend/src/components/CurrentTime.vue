<template>
  <div>
    <h1>Текущее время с сервера: </h1>

    <p v-if="loading" class="text-gray-500">Загрузка...</p>
    <p v-else-if="error" class="text-red-500">Ошибка: {{ error }}</p>
    <p v-else class="text-green-700">Время: {{ time }}</p>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from 'vue'
import { api } from '../services/api'

export default defineComponent({
  setup() {
    const time = ref('')
    const error = ref('')
    const loading = ref(true)

    onMounted(async () => {
      try {
        const response = await api.get('/hello')
        time.value = response.data.time
      } catch (err: any) {
        error.value = err.message || 'Ошибка загрузки'
      } finally {
        loading.value = false
      }
    })

    return { time, error, loading }
  },
})
</script>