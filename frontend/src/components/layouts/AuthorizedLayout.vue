<template>
  <div class="authorized-layout">
    <main>
      <b-dropdown text="Navigation" class="m-md-2 position-absolute top-0 end-0 d-md-none" v-for="(item, index) in menuItems">
        <div v-for="(item, index) in menuItems">
          <b-dropdown-divider v-if="index === 1"></b-dropdown-divider>
          <b-dropdown-item href="#">
            <RouterLink class="nav-link" :to="item.to">
              <i v-if="item.icon" :class="item.icon"></i> {{ item.label }}
            </RouterLink>
          </b-dropdown-item>
        </div>
      </b-dropdown>
      <nav id="sidebarMenu" class="col-md-3 col-lg-2 d-none d-md-block bg-light sidebar" style="">
          <ul class="nav flex-column">
            <li v-for="item in menuItems" class="nav-item">
              <RouterLink class="nav-link" :to="item.to">
                <i v-if="item.icon" :class="item.icon"></i> {{ item.label }}
              </RouterLink>
            </li>
          </ul>
      </nav>
      <div class="content">
        <slot />
      </div>
    </main>
  </div>
</template>

<script>
import { BDropdown, BDropdownItem, BDropdownDivider } from 'bootstrap-vue-next';

export default {
  name: 'AuthorizedLayout',
  components: {
    BDropdown,
    BDropdownItem,
    BDropdownDivider,
  },
  data() {
    return {
      menuItems: [
        { label: 'Progressa', to: '/' },
        { label: 'Dashboard', to: '/dashboard', icon: 'bi bi-house-door-fill fs-6' },
        { label: 'Challenges', to: '/challenges', icon: 'bi bi-trophy-fill fs-6' },
        { label: 'Reminders', to: '/reminders', icon: 'bi bi-bell-fill fs-6' },
        { label: 'Social', to: '/social', icon: 'bi bi-person-circle fs-6' },
      ],
    };
  },
};
</script>

<style scoped>
main {
  display: flex;
  height: 100vh;
}

nav {
  width: 200px;
  background-color: #f5f5f5;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

nav.sidebar .nav-link {
  color: #000;
}

nav.sidebar .nav-link i {
  color: #000;
}

.content {
  flex: 1;
  padding: 20px;
}
</style>