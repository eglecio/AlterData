<template>
  <q-page class="q-pa-sm">
    <tab-Clientes v-if="exibirListagemClientes"/>

    <q-dialog v-model="confirmacaoDesabilitarClientes" persistent>
      <q-card>
        <q-card-section class="row items-center">
          <q-avatar icon="fas fa-users-slash" color="primary" text-color="white" />
          <span class="q-ml-sm text-justify">Desabilitar todos os clientes que não acessaram o app nos últimos 30 dias?</span>
          <span class="q-ml-sm q-mt-md text-justify"><small>Seu cliente não deixará de ter acesso ao app, apenas reduzirá a quantidade de clientes ativos em sua assinatura. Caso seu cliente desabilitado acesse novamente ele será reativado automaticamente.</small></span>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="primary" v-close-popup @click="confirmacaoDesabilitarClientes = false" />
          <q-btn label="Sim, quero desabilitar" dense color="teal" v-close-popup @click="desabilitarTodos" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-footer elevated>
      <q-toolbar class="row">
        <!-- <q-toolbar-title>Footer</q-toolbar-title> -->
        <!-- <q-btn
          color="teal"
          dense
          class="col-10 offset-1"
          label="Desabilitar clientes que não acessam"
          @click="confirmacaoDesabilitarClientes = true"
          >
        </q-btn> -->
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
      confirmacaoDesabilitarClientes: false,
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
