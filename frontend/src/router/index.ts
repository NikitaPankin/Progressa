import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '../pages/HomePage.vue'
import DashboardPage from '../pages/DashboardPage.vue'
import ChallengesPage from '../pages/ChallengesPage.vue'
import RemindersPage from '../pages/RemindersPage.vue'
import SocialPage from '../pages/SocialPage.vue'
import SigninPage from '../pages/SigninPage.vue'
import RegisterPage from '../pages/RegisterPage.vue'
import CurrentTimePage from '../pages/CurrentTimePage.vue'

const routes = [
  { path: '/', component: HomePage },
  { path: '/dashboard', component: DashboardPage },
  { path: '/challenges', component: ChallengesPage },
  { path: '/reminders', component: RemindersPage },
  { path: '/social', component: SocialPage },
  { path: '/signin', component: SigninPage },
  { path: '/register', component: RegisterPage },
  { path: '/time', component: CurrentTimePage },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router;