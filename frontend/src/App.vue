<script setup lang="ts">
import AuthorizedLayout from './components/layouts/AuthorizedLayout.vue'
</script>

<template>
  <AuthorizedLayout>
    <RouterView />
  </AuthorizedLayout>
</template>