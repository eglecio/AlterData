<template>
  <q-page class="q-pa-sm">
    <div class="row q-col-gutter-sm">
      <div class="col-lg-8 col-md-8 col-xs-12 col-sm-12">
        <q-card class="card-bg no-shadow" bordered>
          <q-toolbar class="text-white text-right row">
            <div class="col-2 offset-10">
              <q-btn round flat icon="keyboard_backspace" @click="$router.push(`/usuarios`)"/>
            </div>

          </q-toolbar>
          <q-separator></q-separator>

          <q-card-section class="text-h6 text-white">
            <div class="text-h6" style="text-transform: capitalize;">{{ !estaCadastrando ? usuario.email : "Cadastro de Usuário" }}</div>
            <div class="text-subtitle2" v-show="!estaCadastrando">Criado em {{ usuario.dataCadastro }}</div>
          </q-card-section>
          <q-card-section class="q-pa-sm">
            <q-list class="row">
              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input type="email" dark color="white" dense v-model="usuario.email" label="Email*"
                  autocorrect="off"
                  autocapitalize="off"
                  autocomplete="off"
                  spellcheck="false"
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
                  <q-input :type="showPassword ? 'text' : 'password'" dark color="white" dense v-model="usuario.senha" label="Senha*"
                  autocorrect="off"
                  autocapitalize="off"
                  autocomplete="off"
                  spellcheck="false"
                  :error-message-class="messageClass" style="padding-bottom: 3px !important">
                    <template v-slot:append>
                      <q-icon :name="showPassword ? 'visibility_off' : 'visibility'" @click="showPassword = !showPassword" />
                    </template>
                  </q-input>
                  <q-btn size="md" label="Gerar Senha" color="teal" @click="gerarSenha" class="text-capitalize text-white q-mb-md" />
                </q-item-section>
              </q-item>

              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12 q-mb-md">
                <q-item-section>
                  <q-select
                    dark
                    color="white"
                    dense
                    v-model="perfil"
                    label="Perfil do Usuário*"
                    :options="[
                      { label: 'Visualização', value: 1 },
                      { label: 'Edição', value: 2 },
                      { label: 'Admin', value: 99 }
                    ]"
                    :error-message-class="messageClass"
                  />
                </q-item-section>
              </q-item>


              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12 text-white q-mb-md">
                <q-item-section>
                  <q-item-label caption class="text-white">Status</q-item-label>
                  <q-toggle v-model="status" color="green" :label="status ? 'Ativo' : 'Inativo'" />
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

const opcoesPerfil = [
  { label: 'Visualização', value: 1 },
  { label: 'Edição', value: 2 },
  // { label: 'Admin', value: 99 }
]

export default defineComponent({
  name: "Usuario",

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
      usuario: ref({}),
      estaCadastrando: ref(false),
      password_dict: {},
      status: ref(true),
      perfil: ref('Visualização'),
      opcoesPerfil,
      showPassword: ref(false)
    }
  },

  methods: {
    gerarSenha() {
      const senha = Math.random().toString(36).slice(-8); // exemplo de geração de senha simples
      this.usuario.senha = senha;
    },

    carrregarPorId () {
      const instance = this
      const routeId = this.$route.params.id
      instance.estaCadastrando = !(routeId > 0)
      if (instance.estaCadastrando) {
        return
      }

      api.get(`usuario/${routeId}`)
        .then(response => {
          if (response.data) {
            instance.usuario = response.data
            instance.status = instance.usuario.status === 1
            if (instance.usuario.perfil == 99) {
              instance.opcoesPerfil.push({ label: 'Admin', value: 99 })
              instance.perfil = 'Admin'
            } else {
              instance.perfil = instance.usuario.perfil === 2 ? 'Edição' : 'Visualização'
            }
          }
        })
        .catch(error => {
          console.error(error)
        })
    },

    async salvar () {
      const instance = this
      instance.showPassword = false
      if (!instance.usuario.email) {
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
      instance.usuario.perfil = instance.perfil === 'Admin' ? 99 : instance.perfil === 'Edição' ? 2 : 1
      instance.usuario.status = instance.status ? 1 : 2
      await (
            instance.estaCadastrando
             ? api.post(`usuario`, instance.usuario)
             : api.put(`usuario`, instance.usuario)
        ).then(response => {
          if (response.status === 200) {
            Notify.create({
              message: `Usuário ${ instance.estaCadastrando ? 'cadastrado' : 'atualizado' } com sucesso`,
              color: 'positive',
              position: 'center',
              timeout: 2000,
              onDismiss: () => {
                instance.$router.push(`/usuarios`)
              }
            })
          } else {
            Notify.create({
              message: 'Erro ao atualizar usuário. Verifique os dados e tente novamente.',
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
