<template>
  <q-page class="q-pa-sm">
    <div class="row q-col-gutter-sm">
      <div class="col-lg-8 col-md-8 col-xs-12 col-sm-12">
        <q-card class="card-bg no-shadow" bordered>
          <q-toolbar class="text-white text-right row">
            <div class="col-2 offset-10">
              <q-btn round flat icon="keyboard_backspace" @click="$router.push(`/clientes`)"/>
            </div>

          </q-toolbar>
          <q-separator></q-separator>

          <q-card-section class="text-h6 text-white">
            <div class="text-h6" style="text-transform: capitalize;">{{ !estaCadastrando ? cliente.nome : "Cadastro de Cliente" }}</div>
            <div class="text-subtitle2" v-show="!estaCadastrando">Criado em {{ cliente.dataCadastro }}</div>
          </q-card-section>
          <q-card-section class="q-pa-sm">
            <q-list class="row">
              <!-- para dar um upgrade em ter uma imagem de cliente uso esse template -->
              <!-- <q-item class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <q-item-section side>
                  <q-avatar size="100px">
                    <img src="https://cdn.quasar.dev/img/boy-avatar.png">
                  </q-avatar>
                </q-item-section>
                <q-item-section>
                  <q-btn label="Add Photo" class="text-capitalize" rounded color="info"
                         style="max-width: 120px"></q-btn>
                </q-item-section>
              </q-item> -->

              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="cliente.nome" label="Nome*"
                    :error-message-class="messageClass"
                    :rules="[
                      val => !!val || 'Nome é obrigatório',
                      val => val.length >= 2 || 'Nome deve ter no mínimo 2 caracteres',
                      val => val.length <= 150 || 'Nome deve ter no máximo 150 caracteres',
                      val => /^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$/.test(val) || 'Nome deve conter apenas letras'
                    ]"
                  />
                </q-item-section>
              </q-item>
              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="cliente.email" label="Email*"
                  :error-message-class="messageClass"
                  :rules="[
                    val => !!val || 'Email é obrigatório',
                    val => /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(val) || 'Email inválido'
                  ]"
                  />
                </q-item-section>
              </q-item>
              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="cliente.cpf" label="CPF*"
                  :error-message-class="messageClass"
                  mask="###.###.###-##"
                  unmasked-value
                  :rules="[
                    val => val && val.length > 0 || 'CPF é obrigatório',
                    val => val.length === 11 || 'CPF inválido'
                  ]"
                  />
                </q-item-section>
              </q-item>
              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="cliente.telefone" label="Telefone"
                  :error-message-class="messageClass"
                  mask="(##) #####-####"
                  unmasked-value
                  />
                </q-item-section>
              </q-item>
              <q-item class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" autogrow dense v-model="cliente.endereco" label="Endereço"/>
                </q-item-section>
              </q-item>
              <q-item class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" type="textarea" dense v-model="cliente.observacao" label="Observação"/>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card-section>
          <q-card-actions align="right">
            <q-btn v-if="estaCadastrando" class="text-capitalize bg-positive text-white" @click="salvar">Cadastrar</q-btn>
            <q-btn v-else class="text-capitalize bg-info text-white" @click="salvar">Atualizar</q-btn>
          </q-card-actions>
        </q-card>
      </div>

    </div>
  </q-page>
</template>

<script>
import {defineComponent, ref} from 'vue'
import { api } from 'boot/axios'
import { useQuasar, Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'

export default defineComponent({
  name: "Cliente",

  computed: {
    messageClass() {
      return this.$q.dark.isActive ? 'text-white' : 'text-red'
    }
  },

  mounted () {
    this.carrregarPorId()
  },

  setup() {
    return {
      cliente: ref({}),
      estaCadastrando: ref(false),
      password_dict: {}
    }
  },

  methods: {
    carrregarPorId () {
      const instance = this
      const routeId = this.$route.params.id
      instance.estaCadastrando = !(routeId > 0)
      if (instance.estaCadastrando) {
        return
      }

      api.get(`cliente/${routeId}`)
        .then(response => {
          if (response.data) {
            instance.cliente = response.data
          }
        })
        .catch(error => {
          console.error(error)
        })
    },

    async salvar () {
      const instance = this
      if (!instance.cliente.nome || !instance.cliente.email || !instance.cliente.cpf) {
        Notify.create({
          message: 'Preencha todos os campos obrigatório',
          color: 'negative',
          icon: 'warning',
          position: 'center',
          timeout: 2000
        })
        return
      }

      Loading.show({ spinner: QSpinnerGears })

      await (
            instance.estaCadastrando
             ? api.post(`cliente`, instance.cliente)
             : api.put(`cliente`, instance.cliente)
        ).then(response => {
          if (response.status === 200) {
            Notify.create({
              message: `Cliente ${ instance.estaCadastrando ? 'cadastrado' : 'atualizado' } com sucesso`,
              color: 'positive',
              position: 'center',
              timeout: 2000,
              onDismiss: () => {
                instance.$router.push(`/clientes`)
              }
            })
          } else {
            Notify.create({
              message: 'Erro ao atualizar cliente. Verifique os dados e tente novamente.',
              color: 'negative',
              position: 'center'
            })
          }
        })
        .catch(error => {
          console.error(error)
        })
        .finally(() => {
          Loading.hide()
        })
    }
  }
})
</script>

<style scoped>

.card-bg {
  background-color: #162b4d;
  /* background-color: teal; */
}
/* Estilo global para mensagens de validação */
.q-field--dark .q-field__messages {
  opacity: 1;
  font-weight: 500;
}
</style>
