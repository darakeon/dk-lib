package com.darakeon.dk

import android.app.Activity
import android.content.Context
import android.content.Intent

inline fun <reified T : Activity> Context.redirect () {
	val intent = Intent(this, T::class.java)
	startActivity(intent)
}

inline fun <reified T : Activity> Activity.redirectAndFinish () {
	redirect<T>()
	finish()
}
