import './App.css'
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useEffect, useState } from 'react'

const App = () => {
  const [connection, setConnection] = useState(null)
  const [connectionToken] = useState('just-for-example-unique-id-as-a-token')
  const [user, setUser] = useState('no-user')
  const [message, setMessage] = useState('no-message')

  useEffect(() => {
    const serverUrl = 'https://localhost:7777/chatHub'
    const newConnection = new HubConnectionBuilder()
      .withUrl(serverUrl, {
        accessTokenFactory: () => connectionToken,
      })
      .withAutomaticReconnect()
      .build()

    setConnection(newConnection)
  }, [connectionToken])

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then((result) => {
          console.log('Connected!')
          connection.on('SendDataToClient', (payload) => {
            setUser(payload.user)
            setMessage(payload.message)
          })
        })
        .catch((e) => console.log('Connection failed: ', e))
    }
  }, [connection])

  return (
    <div className='App'>
      <header className='App-header'>
        <p>User is: {user}</p>
        <p>Message is: {message}</p>
      </header>
    </div>
  )
}

export default App
