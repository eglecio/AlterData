<template>
  <q-page class="q-pa-sm">
    <div class="row q-col-gutter-sm  q-py-sm">
      <div class="row col-12" v-if="Object.keys(mensagem_selecionada).length == 0">
        <!-- FLTRO - COMENTEI POR ENQUANTO ESTOU SEM TEMPO, MAS O CODIGO NO WS JA ESTA PRONTO... -->
        <div class="col-12">
          <q-card class="no-border no-shadow bg-transparent">
            <q-card-section class="q-pa-xs">
              <q-input rounded v-model="termoPesquisa" @update:model-value="pesquisar" outlined placeholder="Pesquisar Mensagem (remetente/conteúdo)">
                <template v-slot:append>
                  <q-icon v-if="termoPesquisa === ''" name="search"/>
                  <q-icon v-else name="clear" class="cursor-pointer" @click="termoPesquisa = '';resetar();pesquisar('')"/>
                </template>
              </q-input>
            </q-card-section>
          </q-card>
        </div>
        <div class="col-12 row">
          <q-list ref="scrollTargetRef" :style="'max-height: ' + alturaScroll + 'px; overflow: auto;'" class="rounded-borders col-12" animated separator>
            <q-infinite-scroll ref="infinteScrollRef" @load="obterDados" :offset="alturaScroll" :scroll-target="scrollTargetRef">
                <q-item v-for="(mensagem, index) in mensagens" :key="mensagem.Id" clickable v-ripple style="margin-bottom: 2px; border-bottom: 2px solid #D4D4D4">
                  <q-item-section avatar @click="mensagem_selecionada=mensagem">
                    <q-avatar>
                      <img
                      style="border: solid grey 0px; object-fit: cover;"
                      loading="lazy"
                      :src="mensagem.Foto === undefined || mensagem.Foto === null || mensagem.Foto === 'NULL' || mensagem.Foto.indexOf('user-avatar') > -1 || mensagem.Foto === 'https://s3.us.cloud-object-storage.appdomain.cloud/npanexos/' ? 'img/user-avatar.png' : ('https://lbcloud.meuapp.fit/cdn-cgi/image/width=80/' + mensagem.Foto)"
                      >
                    </q-avatar>
                  </q-item-section>

                  <q-item-section @click="mensagem_selecionada=mensagem">
                    <q-item-label lines="1">{{ mensagem.NomeRemetente }}</q-item-label>
                    <q-item-label caption lines="2">
                      <span class="text-weight-bold" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">{{ mensagem.ConteudoDaMensagem }}</span>
                    </q-item-label>
                  </q-item-section>

                  <q-item-section side @click="mensagem_selecionada=mensagem">
                    <q-item-label lines="1">{{ mensagem.DataEnvio }}</q-item-label>
                  </q-item-section>
                </q-item>

              <template v-slot:loading>
                <div class="row justify-center q-my-md">
                  <q-spinner-dots color="primary" size="40px" />
                </div>
              </template>
            </q-infinite-scroll>
          </q-list>
        </div>
      </div>

      <transition v-else
      class="col-12"
          appear
          enter-active-class="animated bounceInRight">
            <q-card class="no-border no-border"
                    :style="{'height':size['height']-100+'px !important'}">
              <q-toolbar class="text-black ">
                <q-btn round flat class="q-pa-sm">
                  <q-avatar size="80px">
                    <img :src="mensagem_selecionada.Foto === undefined || mensagem_selecionada.Foto === null || mensagem_selecionada.Foto === 'NULL' || mensagem_selecionada.Foto.indexOf('user-avatar') > -1 || mensagem_selecionada.Foto === 'https://s3.us.cloud-object-storage.appdomain.cloud/npanexos/' ? 'img/user-avatar.png' : ('https://lbcloud.meuapp.fit/cdn-cgi/image/width=80/' + mensagem_selecionada.Foto)">
                  </q-avatar>
                </q-btn>

                <q-item class="q-subtitle-1 q-pl-md">
                  <q-item-section>
                    <q-item-label lines="1">Notificação</q-item-label>
                    <q-item-label caption lines="1">
                      <span class="text-weight-bold">{{ mensagem_selecionada.DataEnvio }}</span>
                    </q-item-label>
                    <q-item-label caption lines="1">
                      <!-- <q-badge v-if="mensagem_selecionada.Status === 1" color="blue" text-color="white" label="Ativo" />
                      <q-badge v-else color="red" text-color="white" label="Inativo" /> -->
                    </q-item-label>
                  </q-item-section>
                </q-item>

                <q-space/>

                <q-btn round flat icon="keyboard_backspace" @click="resetar();mensagem_selecionada={}"/>

              </q-toolbar>
              <q-separator></q-separator>

              <div v-for="detail, detail_index in detail_list">
                <contact-detail-item :icon="detail.icon" :text_color="detail.text_color"
                                    :value="mensagem_selecionada[detail['field']]" :label="detail.label"></contact-detail-item>

                <q-separator inset="item" v-if="detail_index!=detail_list.length-1"></q-separator>
              </div>

            </q-card>
      </transition>
    </div>
  </q-page>
</template>

<script>
import {defineComponent, defineAsyncComponent, ref} from 'vue'
import { api } from 'boot/axios'
import { Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'

const detail_list = [
  {
    icon: 'fas fa-user',
    label: 'Remetente',
    field: 'NomeRemetente',
    text_color: 'grey-8'
  },
  {
    icon: 'mail',
    label: 'Mensagem',
    field: 'ConteudoDaMensagem',
    text_color: 'grey-8'
  }
];

export default defineComponent({
  name: 'Mensagem',

  components: {
    ContactDetailItem: defineAsyncComponent(() => import('components/ContactDetailItem.vue'))
  },

  setup() {
    const termoPesquisa = ref('');
    const ultimaPesquisa = ref('');
    const scrollTargetRef = ref(null)
    const alturaScroll = document.documentElement.clientHeight - 100
    // const $q = useQuasar()
    const size = ref({ width: '200px', height: '200px' });

    return {
      mensagens: ref([]),
      mensagem_selecionada: ref({}),
      termoPesquisa,
      ultimaPesquisa,
      scrollTargetRef,
      alturaScroll,
      size,
      detail_list,
      messages: [
        {
          id: 5,
          name: 'Pratik Patel',
          msg: ' -- I\'ll be in your neighborhood doing errands this\n' +
            '            weekend. Do you want to grab brunch?',
          avatar: 'https://avatars2.githubusercontent.com/u/34883558?s=400&v=4',
          time: '10:42 PM'
        }, {
          id: 6,
          name: 'Winfield Stapforth',
          msg: ' -- I\'ll be in your neighborhood doing errands this\n' +
            '            weekend. Do you want to grab brunch?',
          avatar: 'https://cdn.quasar.dev/img/avatar6.jpg',
          time: '11:17 AM'
        }, {
          id: 1,
          name: 'Boy',
          msg: ' -- I\'ll be in your neighborhood doing errands this\n' +
            '            weekend. Do you want to grab brunch?',
          avatar: 'https://cdn.quasar.dev/img/boy-avatar.png',
          time: '5:17 AM'
        }, {
          id: 2,
          name: 'Jeff Galbraith',
          msg: ' -- I\'ll be in your neighborhood doing errands this\n' +
            '            weekend. Do you want to grab brunch?',
          avatar: 'https://cdn.quasar.dev/team/jeff_galbraith.jpg',
          time: '5:17 AM'
        }, {
          id: 3,
          name: 'Razvan Stoenescu',
          msg: ' -- I\'ll be in your neighborhood doing errands this\n' +
            '            weekend. Do you want to grab brunch?',
          avatar: 'https://cdn.quasar.dev/team/razvan_stoenescu.jpeg',
          time: '5:17 AM'
        }
      ],
    }
  },

  methods: {

    async obterDados (index, done) {
      var instance = this

      this.mensagens = this.pagina === 0 ? [] : this.mensagens
      this.pagina = index >= 0 ? index : (this.pagina + 1)

      try {
        var finalizarCarregamento = (possuiRegistros) => {
          setTimeout(() => {
            if (possuiRegistros === false) {
              instance.$refs.infinteScrollRef.stop()
            }
            done()// usado pra desaparecer o spinner...
          }, index < 5 ? 1000 : 400)
        }

        var dados = {
          Pagina: this.pagina,
          TotalPorPagina: 10,
          TermoPesquisa: this.termoPesquisa
        }

        const response = await api.post(`mensagem/listar`, JSON.stringify(dados))
        if (response.data && response.data.Result.length > 0) {
          this.mensagens.push(...response.data.Result)
        }

        if (index >= 0) {
          finalizarCarregamento(response.data.Result.length > 0)
        }

      } catch (error) {
        console.log(error)
      }

      // Loading.hide()
    },

    pesquisar (e) {
      var instance = this
      this.pagina = 0

      setTimeout(async () => {
        if (e === instance.termoPesquisa && instance.termoPesquisa !== '') {
          instance.ultimaPesquisa = instance.termoPesquisa
        } else if (e === '' && instance.ultimaPesquisa !== '') {
          instance.termoPesquisa = ''
          instance.ultimaPesquisa = ''
        }
        instance.mensagens = []
        instance.$refs.infinteScrollRef.reset()
        setTimeout(() => {
          instance.$refs.infinteScrollRef.resume()
        }, 500)
      }, e === '' ? 50 : 1000)
    },

    resetar () {
      this.pagina = 0
      this.mensagens = []
    }

  }

})
</script>
