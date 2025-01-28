<template>
  <q-item clickable v-ripple>
    <q-item-section avatar top>
      <q-avatar :icon="icon" color="grey-2" :text-color="text_color"/>
    </q-item-section>

    <q-item-section>
      <q-item-label lines="1">{{ formatarCampo() }}</q-item-label>
      <q-item-label caption class="text-grey-8">{{ label }}</q-item-label>
    </q-item-section>

  </q-item>
</template>

<script>
import {defineComponent} from 'vue';

export default defineComponent({
  name: "ContactDetailItem",
  props: ['icon', 'text_color', 'value', 'label'],

  methods: {
    formatarCpf (cpf) {
      cpf = cpf.replace(/[^\d]/g, "")//retira os caracteres indesejados...
      return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
    },

    formatarTelefone (telefone) {
      telefone = telefone.replace(/[^\d]/g, "")//retira os caracteres indesejados...
      return telefone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
    },

    formatarCampo() {
      if (this.label === 'CPF') {
        return this.formatarCpf(this.value)
      } else if (this.label === 'Telefone') {
        return this.formatarTelefone(this.value)
      } else {
        return this.value
      }
    }
  }
})
</script>

<style scoped>

</style>
