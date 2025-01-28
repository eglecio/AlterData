<template>
  <div>
    <q-item to="/mensagens" style="max-width: 420px" v-for="msg in messages" :key="msg.Id" clickable v-ripple>
      <q-item-section avatar>
        <q-avatar>
          <!-- <img :src="msg.avatar"> -->
          <img
            style="border: solid grey 0px; object-fit: cover;"
            loading="lazy"
            :src="msg.Foto === undefined || msg.Foto === null || msg.Foto === 'NULL' || msg.Foto.indexOf('user-avatar') > -1 || msg.Foto === 'https://s3.us.cloud-object-storage.appdomain.cloud/npanexos/' ? 'img/user-avatar.png' : ('https://lbcloud.meuapp.fit/cdn-cgi/image/width=80/' + msg.Foto)"
          >
          <!-- <img :src="'https://cdn.quasar.dev/img/avatar6.jpg'"> -->
        </q-avatar>
      </q-item-section>

      <q-item-section>
        <q-item-label>{{ msg.NomeRemetente }}</q-item-label>
        <q-item-label caption lines="1">{{ msg.ConteudoDaMensagem }}</q-item-label>
      </q-item-section>

      <q-item-section side>
        {{ msg.DataEnvio }}
      </q-item-section>
    </q-item>
  </div>
</template>

<script>
import {defineComponent, ref} from 'vue'
import { api } from 'boot/axios'
import { useQuasar, Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'

export default defineComponent({
  name: "Messages",
  setup() {
    return {
      messages: ref([
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
      ]),
    }
  },

  mounted () {
    this.obterUltimasMensagens()
  },

  methods: {
    async obterUltimasMensagens () {
      var instance = this
      Loading.show({ spinner: QSpinnerGears })

      this.messages = []
      try {
        var dados = {
          Pagina: 1,
          TotalPorPagina: 20,
          TermoPesquisa: ''
        }

        const response = await api.post(`mensagem/listar`, JSON.stringify(dados))

        if (response.data && response.data.Result.length > 0) {
          this.messages = response.data.Result
        }
      } catch (error) {
        console.log(error)
      }

      Loading.hide()
    }
  }
})
</script>

<style scoped>

</style>
