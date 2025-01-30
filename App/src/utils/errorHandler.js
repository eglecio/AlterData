import { Notify, LocalStorage } from 'quasar'
import { api } from 'src/boot/axios';
import { useRouter } from 'vue-router'

// Crie um módulo separado para gerenciar a navegação
const navegacaoRouter = {
  router: null,

  // Método para inicializar o router
  initRouter(router) {
    this.router = router
  },

  // Método para navegar
  irParaLogin() {
    if (this.router) {
      this.router.push('/login')
    }
  }
}

export const handleAxiosError = (error, customConfig = {}) => {

  const efetuarLogoutSessaoExpirada = () => {
    Notify.create({
      type: 'negative',
      message: 'Sessão expirada. Por favor, faça login novamente.',
      position: 'center',
      timeout: 3000
    })
    LocalStorage.removeItem('token')
    navegacaoRouter.irParaLogin()
    return
  }

  const erroDeConexao = () => {
    Notify.create({
      type: 'negative',
      message: 'Erro de conexão. Verifique sua internet.',
      position: 'center',
      timeout: 3000,
      ...customConfig
    })
    return Promise.reject(error)
  }

  // Primeiro, verificamos explicitamente se é um erro 401
  if (error.response?.status === 401) {
    efetuarLogoutSessaoExpirada()
    return Promise.reject(error)
  }

  if (!error.response) {  // Provavel erro de conexão...
    const token = LocalStorage.getItem('token')
    if (!token) {
      // navegacaoRouter.irParaLogin()
      return
    }
    // // Efetuo uma chamada para metodo de validacao de TOKEN EXPIRADO. Nao esta tratando o erro 401 e outros de forma correta, sempre ta caindo aqui,
    // // entao vou valido se o token ainda eh valido no WS e senao for eu retorno para o login...
    // api.get('usuario/validarToken', { headers: { Authorization: `Bearer ${token}` } })
    //   .then(response => {
    //     // Se a validação for bem sucedida e retornar false, entao a sessão expirou...
    //     if (response.data === false) {
    //       efetuarLogoutSessaoExpirada()
    //       return
    //     }

    //     erroDeConexao()
    //   })
    //   .catch(() => {
    //     erroDeConexao()
    //   })
    // efetuarLogoutSessaoExpirada()
    // erroDeConexao()
    return Promise.reject(error)
  }

  const { status, data } = error.response

  // Se recebemos uma lista de erros de validação
  if (data && Array.isArray(data)) {
    // console.log('Array de erros detectado:', data)
    data.forEach(erro => {
      Notify.create({
        type: 'warning',
        message: erro.errorMessage || erro.message || erro,
        position: 'center',
        timeout: 4000,
        ...customConfig
      })
    })
    return Promise.reject(error)
  }

  // Tratamento padrão baseado no status HTTP
  const mensagensDeErro = {
    400: 'Dados inválidos. Verifique as informações enviadas.',
    401: 'Acesso não autorizado. Se isso for um engano, por favor faça login novamente.',
    403: 'Acesso negado. Você não tem permissão para esta ação.',
    404: 'Registro não encontrado.',
    405: 'Requisição não permitida ou parâmetros inválidos.',
    422: 'Dados inválidos. Verifique as informações enviadas.',
    429: 'Muitas requisições. Aguarde um momento.',
    500: 'Erro interno do servidor.',
    503: 'Serviço indisponível no momento.'
  }

  const mensagemPadrao = 'Ocorreu um erro inesperado.'
  const mensagem = mensagensDeErro[status] || mensagemPadrao

  Notify.create({
    type: 'negative',
    message: mensagem,
    position: 'center',
    timeout: 4000,
    ...customConfig
  })
}

// Configuração global do Axios com interceptadores...
export const setupAxiosInterceptors = (axios) => {
  axios.interceptors.request.use(
    config => {
      const token = LocalStorage.getItem('token')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    },
    error => Promise.reject(error)
  )

  // Interceptador de resposta modificado para debug se precisar...
  axios.interceptors.response.use(
    response => response,
    error => {
      // // #### Log para debug ####
      // console.log('Axios interceptor error:', {
      //   status: error.response?.status,
      //   data: error.response?.data,
      //   error: error
      // });

      handleAxiosError(error)
      return Promise.reject(error)
    }
  )
}

export { navegacaoRouter }
