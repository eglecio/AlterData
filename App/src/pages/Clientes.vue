<template>
  <q-page class="q-pa-sm">

    <tab-Clientes v-if="exibirListagemClientes"/>

    <q-footer elevated>
      <q-toolbar class="row">
        <q-btn
          fab
          color="teal-7"
          icon="fas fa-plus" padding="sm"
          class="absolute"
          style="top: 0; right: 12px; transform: translateY(-50%);"
          to="/cliente/novo"
        />
      </q-toolbar>
    </q-footer>
  </q-page>
</template>

<script>
import {defineComponent,defineAsyncComponent} from 'vue'
import { api } from 'boot/axios'
import { Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'

export default defineComponent({
  name: 'Clientes',
  components: {
    TabClientes: defineAsyncComponent(() => import('components/tabs/TabClientes.vue')),
  },

  data () {
    return {
      exibirListagemClientes: true
    }
  },

  methods: {

    async desabilitarTodos () {
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.post(`cliente/desabilitarinativos`)
        if (response.data && response.data.Result && response.data.Result === true) {
          this.exibirListagemClientes = false
          Notify.create({
            message: 'Clientes desativados com sucesso',
            color: 'positive',
            icon: 'check',
            position: 'center',
            timeout: 2500
          })
          var instance = this
          setTimeout(() => {
            instance.exibirListagemClientes = true
          }, 500)

        } else {
          Notify.create({
            message: 'Não foi possível desabilitar seus clientes. Por favor tente novamente...',
            color: 'negative',
            icon: 'warning',
            position: 'center',
            timeout: 3000
          })
        }


      } catch (error) {
        Notify.create({
          message: 'Ops! Não foi possível desabilitar seus clientes, tente novamente mais tarde...',
          color: 'negative',
          icon: 'warning',
          position: 'center',
          timeout: 3000
        })
        console.log(error)
      }

      Loading.hide()
    },
  }

})
</script>
