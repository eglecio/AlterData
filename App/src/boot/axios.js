import { boot } from 'quasar/wrappers'
import axios from 'axios'
import { handleAxiosError } from 'src/utils/errorHandler'
import { LocalStorage } from 'quasar'

// Be careful when using SSR for cross-request state pollution
// due to creating a Singleton instance here;
// If any client changes this (global) instance, it might be a
// good idea to move this instance creation inside of the
// "export default () => {}" function below (which runs individually
// for each client)

// const api = axios.create({ baseURL: 'https://api.example.com' })
const api = axios.create({
  baseURL: 'https://localhost:7188',
  headers: {
    'Content-Type': 'application/json;charset=UTF-8'
  }
})

// Adicionando console.log para debug
api.interceptors.response.use(
  response => {
    // console.log('Sucesso na requisição:', response)
    return response
  },
  error => {
    // console.log('Erro capturado no interceptor:', error)
    handleAxiosError(error)
    return Promise.reject(error)
  }
)

// Interceptor para adicionar o token em todas as requisições
api.interceptors.request.use(config => {
  const token = LocalStorage.getItem('token')
  if (token) {
    // console.log(token)
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})


export default boot(({ app }) => {
  // for use inside Vue files (Options API) through this.$axios and this.$api

  app.config.globalProperties.$axios = axios
  // ^ ^ ^ this will allow you to use this.$axios (for Vue Options API form)
  //       so you won't necessarily have to import axios in each vue file

  app.config.globalProperties.$api = api
  // ^ ^ ^ this will allow you to use this.$api (for Vue Options API form)
  //       so you can easily perform requests against your app's API
})

export { api }
