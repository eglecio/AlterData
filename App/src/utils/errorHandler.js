import { Notify } from 'quasar'

export const handleAxiosError = (error, customConfig = {}) => {
  // Se não há resposta, provavelmente é um erro de conexão
  if (!error.response) {
    // console.log('Erro sem resposta detectado')
    Notify.create({
      type: 'negative',
      message: 'Erro de conexão. Verifique sua internet.',
      position: 'center',
      timeout: 3000,
      ...customConfig
    })
    return
  }

  const { status, data } = error.response
  // console.log('Status:', status, 'Data:', data)

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
    return
  }

  // Tratamento padrão baseado no status HTTP
  const mensagensDeErro = {
    400: 'Dados inválidos. Verifique as informações enviadas.',
    401: 'Acesso não autorizado. Se isso for um engano, por favor faça login novamente.',
    403: 'Acesso negado. Você não tem permissão para esta ação.',
    404: 'Registro não encontrado.',
    405: 'Requisição não permitda ou parâmetros inválidos.',
    422: 'Dados inválidos. Verifique as informações enviadas.',
    429: 'Muitas requisições. Aguarde um momento.',
    500: 'Erro interno do servidor.',
    503: 'Serviço indisponível no momento.'
  }

  const mensagemPadrao = 'Ocorreu um erro inesperado.'
  const mensagem = mensagensDeErro[status] || mensagemPadrao

  // console.log('Mensagem de erro a ser exibida:', mensagem)

  Notify.create({
    type: 'negative',
    message: mensagem,
    position: 'center',
    timeout: 4000,
    ...customConfig
  })
}

// Configuração global do Axios
export const setupAxiosInterceptors = (axios) => {
  axios.interceptors.response.use(
    response => response,
    error => {
      handleAxiosError(error)
      return Promise.reject(error)
    }
  )
}
