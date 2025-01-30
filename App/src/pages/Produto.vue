<template>
  <q-page class="q-pa-sm">
    <div class="row q-col-gutter-sm">
      <div class="col-lg-8 col-md-8 col-xs-12 col-sm-12">
        <q-card class="card-bg no-shadow" bordered>
          <q-toolbar class="text-white text-right row">
            <div class="col-2 offset-10">
              <q-btn round flat icon="keyboard_backspace" @click="$router.push(`/produtos`)"/>
            </div>

          </q-toolbar>
          <q-separator></q-separator>

          <q-card-section class="text-h6 text-white">
            <div class="text-h6" style="text-transform: capitalize;">{{ !estaCadastrando ? produto.nome : "Cadastro de Produto" }}</div>
            <div class="text-subtitle2" v-show="!estaCadastrando">Criado em {{ produto.dataCadastro }}</div>
          </q-card-section>
          <q-card-section class="q-pa-sm">
            <q-list class="row">
              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="produto.nome" label="Nome*"
                    :error-message-class="messageClass"
                    :rules="[
                      val => !!val || 'Nome é obrigatório',
                      val => val.length >= 3 || 'Nome deve ter no mínimo 3 caracteres',
                      val => val.length <= 100 || 'Nome deve ter no máximo 100 caracteres'
                    ]"
                  />
                </q-item-section>
              </q-item>

              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="produto.quantidadeEstoque" label="Quantidade em Estoque*"
                    :error-message-class="messageClass"
                    :rules="[
                      val => !!val || 'Quantidade em estoque é obrigatória'
                    ]"
                    type="number"
                    mask="#.##"
                    fill-mask="0"
                    reverse-fill-mask
                    input-class="text-right"
                  />
                </q-item-section>
              </q-item>
              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="produto.valorCusto" label="Valor de Custo* (R$)"
                    :error-message-class="messageClass"
                    :rules="[
                      val => !!val || 'Valor de custo é obrigatório'
                    ]"
                    mask="#.##"
                    fill-mask="0"
                    reverse-fill-mask
                    input-class="text-right">
                    <!-- <template v-slot:append>
                      R$
                      <q-icon name="attach_money" />
                    </template> -->
                  </q-input>
                </q-item-section>
              </q-item>
              <q-item class="col-lg-6 col-md-6 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" dense v-model="produto.valorVenda" label="Valor de Venda* (R$)"
                    :error-message-class="messageClass"
                    :rules="[
                      val => !!val || 'Valor de venda é obrigatório'
                    ]"
                    mask="#.##"
                    fill-mask="0"
                    reverse-fill-mask
                    input-class="text-right">
                    <!-- <template v-slot:prepend>
                      <q-icon name="attach_money" />
                    </template> -->
                  </q-input>
                </q-item-section>
              </q-item>

              <q-item class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                <q-item-section>
                  <q-input dark color="white" type="textarea" dense v-model="produto.observacao" label="Observação"/>
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
import { Loading, Notify, QSpinnerGears } from 'quasar'

export default defineComponent({
  name: "Produto",

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
      rota: ref('produto'),
      produto: ref({ }),
      estaCadastrando: ref(false)
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

      api.get(`${instance.rota}/${routeId}`)
        .then(response => {
          if (response.data) {
            instance.produto = response.data
          }
         })
        .catch(error => {
          console.error(error)
        })
    },

    async salvar () {
      const instance = this
      if (!instance.produto.nome || !instance.produto.quantidadeEstoque || !instance.produto.valorCusto || !instance.produto.valorVenda) {
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
             ? api.post(`${instance.rota}`, instance.produto)
             : api.put(`${instance.rota}`, instance.produto)
        ).then(response => {
          if (response.status === 200) {
            Notify.create({
              message: `Produto ${ instance.estaCadastrando ? 'cadastrado' : 'atualizado' } com sucesso`,
              color: 'positive',
              position: 'center',
              timeout: 1500,
              onDismiss: () => {
                instance.$router.push(`/produtos`)
              }
            })
          } else {
            Notify.create({
              message: 'Erro ao salvar produto. Verifique os dados e tente novamente.',
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
