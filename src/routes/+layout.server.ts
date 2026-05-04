import { PUBLIC_API_URL } from '$lib/environment';
import { redirect } from '@sveltejs/kit';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ fetch }) => {
  const res = await fetch(`${PUBLIC_API_URL}/user/@me`);

  if (!res.ok) {
    throw redirect(302, `${PUBLIC_API_URL}/auth/signin/microsoft`);
  }

  const user = await res.json();

  return { user };
};