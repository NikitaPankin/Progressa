<template>
  <div>Welcomeback, Alex</div>
  <button @click="logout">Logout</button>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router";
import { removeTokens } from "../utils/tokenStorage";

const router = useRouter();

function logout() {
  removeTokens();
  router.push("/");
}
</script>