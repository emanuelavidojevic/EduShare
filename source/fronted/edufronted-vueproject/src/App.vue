<script setup>
import { ref, onMounted } from 'vue'

const nivoi = ref([]) // lista nivoa

onMounted(async () => {
const response = await fetch('http://localhost:5002/api/levels')
nivoi.value = await response.json()
})
</script>

<template>
  <h1>Nivoi igre</h1>

  <ul>
    <li v-for="nivo in nivoi" :key="nivo.id">
      {{ nivo.ime }}
    </li>
  </ul>
</template>