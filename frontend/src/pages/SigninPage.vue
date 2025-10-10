<template>
  <div class="login-form">
    <form @submit.prevent="login">
      <div class="form-group">
        <label for="username">Login:</label>
        <input type="text" id="username" v-model="username" placeholder="Enter login"/>
      </div>

      <div class="form-group">
        <label for="password">Password:</label>
        <input type="password" id="password" v-model="password" placeholder="Enter password"/>
      </div>

      <button type="submit" :disabled="!isFormPrepared">Enter</button>

      <p v-if="error" class="error-message">{{ error }}</p>
    </form>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { api } from "../services/api";
import { saveTokens } from "../utils/tokenStorage";

const username = ref("");
const password = ref("");
const error = ref("");
const router = useRouter();

const isFormPrepared = computed(() => username.value.trim() !== "" && password.value.trim() !== "");

async function login() {
  try {
    const response = await api.post("/session/signin", {
      login: username.value,
      password: password.value,
    });

    const { accessToken, refreshToken } = response.data;
    saveTokens({ accessToken, refreshToken });

    router.push("/dashboard");
  } catch (err) {
    if (err.response && err.response.data) {
      error.value = err.response.data;
    } else {
      error.value = "An unknown error occurred. Please try again.";
    }
  }
}
</script>

<style scoped>
.error-message {
  color: red;
}
</style>