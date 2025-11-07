<template>
  <div class="register-form">
    <form @submit.prevent="register">

      <div class="form-group">
        <label for="email">Email:</label>
        <input type="email" id="email" v-model="email" placeholder="Enter email"/>
        <p v-if="email && !isEmailValid" class="error-message">Invalid email format</p>
      </div>
    
      <div class="form-group">
        <label for="username">User name:</label>
        <input type="text" id="username" v-model="username" placeholder="Enter name"/>
      </div>

      <div class="form-group">
        <label for="password">Password:</label>
        <input type="password" id="password" v-model="password" placeholder="Enter password"/>
        <ul class="validation-list" v-if="password">
          <li v-if="!passwordValidation.hasMinLength" class="error-message">Password must have at least 8 characters</li>
          <li v-if="!passwordValidation.hasUppercase" class="error-message">Password must have at least one uppercase letter</li>
          <li v-if="!passwordValidation.hasLowercase" class="error-message">Password must have at least one lowercase letter</li>
          <li v-if="!passwordValidation.hasDigit" class="error-message">Password must have at least one digit</li>
          <li v-if="!passwordValidation.hasSpecial" class="error-message">Password must have at least one special symbol</li>
        </ul>
      </div>

      <div class="form-group">
        <label for="repitPassword">Repit password:</label>
        <input type="password" id="repitPassword" v-model="repitPassword" placeholder="Enter password"/>
        <p v-if="repitPassword && !passwordRepitedCorrectly" class="error-message">Incorect password</p>
      </div>

      <button type="submit" :disabled="!canSubmit">Register</button>

      <p v-if="error" class="error-message">{{ error }}</p>

      <RouterLink class="nav-link" to= "/signin">Already have an account?</RouterLink>
    </form>
  </div>
</template>

<script setup>
import { ref, computed } from "vue";
import { useRouter } from "vue-router";
import { api } from "../services/api";
import { saveTokens } from "../utils/tokenStorage";
import { validatePassword } from "../utils/Validators/passwordValidator";

const email = ref("");
const username = ref("");
const password = ref("");
const repitPassword = ref("");
const error = ref("");
const router = useRouter();

const isEmailValid = computed(() => /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/.test(email.value));

const passwordValidation = computed(() => validatePassword(password.value));
const passwordRepitedCorrectly = computed(() => password.value == repitPassword.value);

const isPasswordValid = computed(() =>
  passwordValidation.value.isValid &&
  passwordRepitedCorrectly.value
);

const canSubmit = computed(() =>
  username.value.trim() !== "" &&
  isEmailValid.value &&
  isPasswordValid.value
);

async function register() {
  try {
    const response = await api.post("/session/register", {
      email: email.value,
      name : username.value,
      password: password.value
    });

    if (response.data.requiresEmailConfirmation) {
      alert("Check your email to confirm your account!");
      router.push("/signin");
    }
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

.nav-link{
    color: blue;
}
</style>