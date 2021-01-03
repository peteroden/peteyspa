import React, { useState, useEffect, useReducer, Reducer, FormEventHandler } from 'react'

const Header = () => {
	return (
		<header>
			<nav>
				<img src='/logo-new.png' alt='Great People logo' height='90'/>
				<ul>
					<li>Home</li>
				</ul>
			</nav>
			<h2>Find and get to know Great People</h2>
			<p>Fill out your profile so others can learn more about you.</p>
		</header>
	)
}

interface Profile {
	id: string;
	email: string;
	firstName: string;
	lastName: string;
	about: string;
	skills: string;
	interests: string;
}

const ProfileForm: React.FC<{ profile: Profile, dispatch: React.Dispatch<MainReducerActions> }> = ({ profile, dispatch }) => {
	const [submitting, setSubmitting] = useState(false)

	const handleSubmit: FormEventHandler = (event) => {
		event.preventDefault()
		setSubmitting(true)

		fetch('/api/person/'+profile.id, {
			method: 'POST',
			headers: {
					'Content-Type': 'application/json',
				},
			body: JSON.stringify(profile)
		}).then( res => {
			setTimeout(() => {
				alert('Submitted Profile Update form!')
				setSubmitting(false)
			}, 3 * 1000)
		})
	}

	const handleChange = <K extends keyof Profile>(key: K, value: Profile[K]) => {
		dispatch({
			type: 'set profile',
			payload: {
				...profile,
				[key]: value
			}
		})
	}

	return (
		<form onSubmit={event => handleSubmit(event)}>
			<header>
				<h3>Update Profile</h3>
			</header>
			<label>
				User ID:
				<input
					disabled
					type='text'
					value={profile.id}
					onChange={event => handleChange('id', event.target.value)}
				/>
			</label>
			<label>
				Email:
				<input
					disabled
					type='text'
					value={profile.email}
					onChange={event => handleChange('email', event.target.value)}
				/>
			</label>
			<label>
				First Name:
				<input
					type='text'
					value={profile.firstName}
					onChange={event => handleChange('firstName', event.target.value)}
				/>
			</label>
			<label>
				Last Name:
				<input
					type='text'
					value={profile.lastName}
					onChange={event => handleChange('lastName', event.target.value)}
				/>
			</label>
			<label>
				About Yourself
				<textarea
					rows={6}
					cols={40}
					value={profile.about}
					disabled={submitting}
					onChange={event => handleChange('about', event.target.value)}
				/>
			</label>
			<label>
				What are your skills?
				<textarea
					rows={6}
					cols={40}
					value={profile.skills}
					disabled={submitting}
					onChange={event => handleChange('skills', event.target.value)}
				/>
			</label>
			<label>
				What are your interests?
				<textarea
					rows={6}
					cols={40}
					value={profile.interests}
					disabled={submitting}
					onChange={event => handleChange('interests', event.target.value)}
				/>
			</label>
			<input
				type="submit"
				value="Submit"
			/>
		</form>
	)
}

type MainReducerState = {
	profile?: Profile,
	error?: string
}

type MainReducerActions =
	| { type: 'set profile', payload: Profile }
	| { type: 'set error', payload: string }
	| { type: 'clear error' }

const Main: React.FC = () => {
	const [ state, dispatch ] = useReducer<Reducer<MainReducerState, MainReducerActions>>((state, action) => {
		switch (action.type) {
			case 'set profile':
				return { ...state, profile: action.payload }
			case 'set error':
				return { ...state, error: action.payload }
			case 'clear error':
				return { ...state, error: undefined }
		}
	}, {})

	useEffect(() => {
		fetch(`/.auth/me`)
			.then( async (res) => {
				let { clientPrincipal } = await res.json()
				let user = clientPrincipal;

				fetch(`/api/person/${user.userId}`)
				.then(res => {
					if (!res.ok) {
						throw Error(res.statusText)
					}
					return res.json()
				})
				.then(res => {
					dispatch({
						type: 'set profile',
						payload: res
					})
				})
				.catch(err => {
					switch (err.message) {
						case 'Not Found':
							dispatch({
								type: 'set error',
								payload: 'User Not Found'
							})
							break
						default:
							dispatch({
								type: 'set error',
								payload: err.message
							})
					}
				})
			})
	}, [])

	return (
		<main>
			<hr></hr>
			<h2>Profile</h2>
			<pre><code>{JSON.stringify(state.error || state.profile, undefined, 2)}</code></pre>
			<section>
				{
					state.profile ? (
						<ProfileForm profile={state.profile} dispatch={dispatch} />
					) : (
						<p><a href="/login">Authenticate to view your profile</a></p>
					)
				}
			</section>
		</main>
	)
}

const Footer = () => {
	return (
		<footer>
			<hr/>
			<p>Created by Ethan Arrowood & Pete Roden</p>
		</footer>
	)
}

function App() {
	return (
		<>
			<Header/>
			<Main/>
			<Footer/>
		</>
	)
}

export default App